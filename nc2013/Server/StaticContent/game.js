var Game = Base.extend({
	constructor: function (options) {
		this.programs = options.programs;
		this.memory = options.memory;
		this.$currentStep = options.$currentStep;
	},
	load: function () {
		var that = this;
		return server.get("debugger/state")
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
			if (programStartInfo)
				programStartInfos.push(programStartInfo);
		}
		if (programStartInfos.length == 0)
			return $.Deferred().reject("At least one program should not be empty");

		var that = this;
		return server.post("debugger/start", programStartInfos)
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
		return server.get("debugger/step", { count: stepCount })
			.pipe(function (stepResponse) {
				if (stepResponse.gameState) {
					return that._setGameState(stepResponse.gameState);
				}
				else if (stepResponse.diff) {
					that.$currentStep.text(stepResponse.diff.currentStep);
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
			this.memory.reset();
			for (var i = 0; i < this.programs.length; ++i) {
				this.programs[i].reset();
			}
			return "reset";
		} else {
			this.$currentStep.text(gameState.currentStep);
			this.memory.setCellStates(gameState.memoryState);
			for (var i = 0; i < gameState.programStates.length; ++i)
				this.programs[i].setProgramState(gameState.programStates[i]);
			if (gameState.gameOver) {
				if (gameState.winner != null)
					this.programs[gameState.winner].win();
				else
					for (var i = 0; i < this.programs.length; ++i)
						this.programs[i].draw();
				return "gameover";
			}
			return "playing";
		}
	}
});

var GameRunner = Base.extend({
	constructor: function (options) {
		this.game = options.game;
		this.onGameStarted = options.onGameStarted;
		this.onGameError = options.onGameError;
	},
	_play: function (options) {
		options = $.extend({ requirePlaying: true }, options);

		var that = this;
		function nextAction(gameRunStatus) {
			var result;
			if (options.requirePlaying && gameRunStatus != "playing") {
				result = that.game.start();
				that.onGameStarted && that.onGameStarted();
			} else {
				result = $.when(gameRunStatus);
			}
			if (options.action)
				result = result.pipe(function () {
					return options.action(that.game);
				});
			return result;
		}

		return this.gameQueue = (this.gameQueue || this.game.load())
			.pipe(nextAction, nextAction)
			.done(function () {
				that.onGameError && that.onGameError(null);
			})
			.fail(function (err) {
				that.pause();
				that.onGameError && that.onGameError(err);
			});
	},
	load: function () {
		this.pause();
		this._play({ requirePlaying: false });
	},
	reset: function () {
		this.pause();
		this._play({
			requirePlaying: false,
			action: function (game) {
				return game.reset();
			}
		});
	},
	step: function (stepCount) {
		this.pause();
		this._play({
			action: function (game) {
				return game.step(stepCount);
			}
		});
	},
	stepToEnd: function () {
		this.pause();
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
