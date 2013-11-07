var Game = Base.extend({
	constructor: function (options) {
		this.programs = options.programs;
		this.memory = options.memory;
		this.breakpoints = options.breakpoints;
		this.$currentStep = options.$currentStep;
	},
	load: function () {
		var that = this;
		return server.get("debugger/reset")
			.pipe(function () {
				return server.get("debugger/state");
			})
			.pipe(function (debuggerState) {
				for (var i = 0; i < that.programs.length; ++i)
					that.programs[i].setProgramStartInfo(debuggerState.programStartInfos && debuggerState.programStartInfos[i]);
				that.breakpoints.setBreakpoints(debuggerState.breakpoints);
				return {
					gameRunStatus: debuggerState.gameState ? that._setGameState(debuggerState.gameState) : that._resetState()
				};
			});
	},
	start: function () {
		var programStartInfos = [];
		for (var i = 0; i < this.programs.length; ++i) {
			var programStartInfo = this.programs[i].getProgramStartInfo();
			if (!programStartInfo)
				return $.Deferred().reject("Program text should not be empty");
			programStartInfos.push(programStartInfo);
		}
		var that = this;
		return server.post("debugger/start", JSON.stringify(programStartInfos))
			.pipe(function (gameState) {
				return {
					gameRunStatus: that._setGameState(gameState)
				};
			});
	},
	_handleStepResponse: function (stepResponse) {
		this.breakpoints.stoppedOnBreakpoint(stepResponse.stoppedOnBreakpoint);
		if (stepResponse.gameState) {
			return {
				gameRunStatus: this._setGameState(stepResponse.gameState),
				stoppedOnBreakpoint: stepResponse.stoppedOnBreakpoint
			};
		}
		else if (stepResponse.diff) {
			return {
				gameRunStatus: this._applyDiff(stepResponse.diff),
				stoppedOnBreakpoint: stepResponse.stoppedOnBreakpoint
			};
		} else
			throw "stepResponse.diff or stepResponse.gameState should not be empty";
	},
	stepToEnd: function () {
		var that = this;
		return server.get("debugger/step/end")
			.pipe(function (stepResponse) {
				return that._handleStepResponse(stepResponse);
			});
	},
	step: function (stepCount) {
		var that = this;
		return server.get("debugger/step", { count: stepCount, currentStep: this.currentStep })
			.pipe(function (stepResponse) {
				return that._handleStepResponse(stepResponse);
			});
	},
	reset: function () {
		return this.load();
	},
	_applyDiff: function (diff) {
		this.currentStep = diff.currentStep;
		this.$currentStep.text(diff.currentStep);
		if (diff.memoryDiffs)
			this.memory.applyDiffs(diff.memoryDiffs);
		if (diff.programStateDiffs)
			for (var i = 0; i < diff.programStateDiffs.length; ++i) {
				var programStateDiff = diff.programStateDiffs[i];
				this.programs[programStateDiff.program].applyDiff(programStateDiff);
			}
		for (var i = 0; i < this.programs.length; ++i)
			if (diff.currentProgram == i)
				this.programs[i].nextProgram();
		if (diff.gameOver) {
			if (diff.winner != null)
				this.programs[diff.winner].win();
			else
				for (var i = 0; i < this.programs.length; ++i)
					this.programs[i].draw();
			return "gameover";
		}
		for (var i = 0; i < this.programs.length; ++i)
			this.programs[i].plays();
		return "playing";
	},
	_resetState: function () {
		this.$currentStep.text("");
		this.currentStep = undefined;
		this.memory.reset();
		for (var i = 0; i < this.programs.length; ++i) {
			this.programs[i].reset();
		}
		return "reset";
	},
	_setGameState: function (gameState) {
		this.$currentStep.text(gameState.currentStep);
		this.currentStep = gameState.currentStep;
		this.memory.setCellStates(gameState.memoryState);
		for (var i = 0; i < gameState.programStates.length; ++i) {
			this.programs[i].setProgramState(gameState.programStates[i]);
			if (gameState.currentProgram == i)
				this.programs[i].nextProgram();
		}
		if (gameState.gameOver) {
			if (gameState.winner != null)
				this.programs[gameState.winner].win();
			else
				for (var i = 0; i < this.programs.length; ++i)
					this.programs[i].draw();
			return "gameover";
		}
		for (var i = 0; i < this.programs.length; ++i)
			this.programs[i].plays();
		return "playing";
	}
});

var GameRunner = Base.extend({
	constructor: function (options) {
		this.game = options.game;
		this.onGameRunStatusChanged = options.onGameRunStatusChanged;
		this.onGameError = options.onGameError;
	},
	_play: function (options) {
		options = $.extend({ requirePlaying: true, singleAction: true }, options);

		if (options.singleAction) {
			if (this.speed)
				this.pause();
			else if (this.gameQueue && !this.gameQueue.isRejected() && !this.gameQueue.isResolved())
				return $.Deferred().reject("busy");
		}

		var that = this;
		function nextAction(status) {
			var result, justStarted = false;
			if (options.requirePlaying && status.gameRunStatus != "playing") {
				result = that.game.start();
				justStarted = true;
			} else
				result = $.when(status);
			if (status.stoppedOnBreakpoint) {
				status.stoppedOnBreakpoint = null;
				that.pause();
			}
			if (options.action)
				result = result.pipe(function () {
					return options.action(that.game, justStarted) || status;
				});
			return result;
		}

		return this.gameQueue = (this.gameQueue || this.game.load())
			.pipe(nextAction, nextAction)
			.done(function (status) {
				that.onGameRunStatusChanged && that.onGameRunStatusChanged(status.gameRunStatus);
				that.onGameError && that.onGameError(null);
			})
			.fail(function (err) {
				that.pause();
				that.onGameError && that.onGameError(err);
			});
	},
	load: function () {
		this._play({
			requirePlaying: false
		});
	},
	reset: function () {
		this._play({
			requirePlaying: false,
			action: function (game) {
				return game.reset();
			}
		});
	},
	step: function (stepCount) {
		this._play({
			action: function (game, justStarted) {
				return game.step(stepCount + (justStarted ? -1 : 0));
			}
		});
	},
	stepToEnd: function () {
		this._play({
			action: function (game) {
				return game.stepToEnd();
			}
		});
	},
	run: function (speed) {
		if (this.speed != speed) {
			var that = this;
			function iteration() {
				if (that.speed)
					that._play({
						singleAction: false,
						action: function (game, justStarted) {
							if (that.speed)
								return game.step(that.speed + (justStarted ? -1 : 0)).done(function (status) {
									if (status.gameRunStatus == "playing")
										setTimeout(iteration, 0);
									else
										that.pause();
								});
						}
					});
			}
			if (this.speed)
				this.speed = speed;
			else {
				this.speed = speed;
				iteration();
			}
		}
	},
	pause: function () {
		this.speed = 0;
	}
});
