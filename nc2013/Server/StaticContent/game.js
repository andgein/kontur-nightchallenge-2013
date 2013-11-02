var Game = Base.extend({
	constructor: function (options) {
		this.programs = options.programs;
		this.memory = options.memory;
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
				return that._setGameState(debuggerState.gameState);
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
				return that._setGameState(gameState);
			});
	},
	stepToEnd: function () {
		var that = this;
		return server.get("debugger/step/end")
			.pipe(function (gameState) {
				return that._setGameState(gameState);
			});
	},
	step: function (stepCount) {
		var that = this;
		return server.get("debugger/step", { count: stepCount, currentStep: this.currentStep })
			.pipe(function (stepResponse) {
				if (stepResponse.gameState) {
					return that._setGameState(stepResponse.gameState);
				}
				else if (stepResponse.diff) {
					that.currentStep = stepResponse.diff.currentStep;
					that.$currentStep.text(stepResponse.diff.currentStep);
					for (var i = 0; i < that.programs.length; ++i)
						this.programs[i].current(stepResponse.diff.currentProgram == i);
					if (stepResponse.diff.memoryDiffs)
						that.memory.applyDiffs(stepResponse.diff.memoryDiffs);
					if (stepResponse.diff.programStateDiffs)
						for (var i = 0; i < stepResponse.diff.programStateDiffs.length; ++i) {
							var programStateDiff = stepResponse.diff.programStateDiffs[i];
							that.programs[programStateDiff.program].applyDiff(programStateDiff);
						}
					if (stepResponse.diff.gameOver) {
						if (stepResponse.diff.winner != null)
							that.programs[stepResponse.diff.winner].win();
						else
							for (var i = 0; i < that.programs.length; ++i)
								that.programs[i].draw();
						return "gameover";
					}
					for (var i = 0; i < that.programs.length; ++i)
						that.programs[i].plays();
					return "playing";
				}
			});
	},
	reset: function () {
		var that = this;
		return server.get("debugger/reset")
			.pipe(function () {
				return that._setGameState(null);
			});
	},
	_setGameState: function (gameState) {
		if (!gameState) {
			this.$currentStep.text("");
			this.currentStep = undefined;
			this.memory.reset();
			for (var i = 0; i < this.programs.length; ++i) {
				this.programs[i].reset();
			}
			return "reset";
		} else {
			this.$currentStep.text(gameState.currentStep);
			this.currentStep = gameState.currentStep;
			this.memory.setCellStates(gameState.memoryState);
			for (var i = 0; i < gameState.programStates.length; ++i) {
				this.programs[i].setProgramState(gameState.programStates[i]);
				this.programs[i].current(gameState.currentProgram == i);
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
	}
});

var GameRunner = Base.extend({
	constructor: function (options) {
		this.game = options.game;
		this.onGameRunStatusChanged = options.onGameRunStatusChanged;
		this.onGameStarted = options.onGameStarted;
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
		function nextAction(gameRunStatus) {
			var result, justStarted = false;
			if (options.requirePlaying && gameRunStatus != "playing") {
				result = that.game.start();
				justStarted = true;
				that.onGameStarted && that.onGameStarted();
			} else {
				result = $.when(gameRunStatus);
			}
			if (options.action)
				result = result.pipe(function () {
					return options.action(that.game, justStarted);
				});
			return result;
		}

		return this.gameQueue = (this.gameQueue || this.game.load())
			.pipe(nextAction, nextAction)
			.done(function (gameRunStatus) {
				that.onGameRunStatusChanged && that.onGameRunStatusChanged(gameRunStatus);
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
						action: function (game) {
							if (that.speed)
								return game.step(that.speed).done(function (gameRunStatus) {
									if (gameRunStatus == "playing")
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
