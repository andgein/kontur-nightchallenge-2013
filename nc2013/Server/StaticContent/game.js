var Game = Base.extend({
	constructor: function (programs, memory) {
		this.programs = programs;
		this.memory = memory;
	},
	start: function () {
		var that = this;
		var programStartInfos = $.map(this.programs, function (program) { return program.programStartInfo; });
		return server.post("start", programStartInfos)
			.pipe(function (gameId) {
				that.gameId = gameId;
				return server.get("state", { gameId: gameId });
			})
			.pipe(function (gameState) {
				that.memory.setCellStates(gameState.memoryState);
				for (var i = 0; i < that.programs.length; ++i) {
					that.programs[i].setProgramState(gameState.programStates[i]);
				}
			});
	},
	step: function (stepCount) {
		var that = this;
		return server.get("step", { gameId: this.gameId, stepCount: stepCount })
			.pipe(function (diff) {
				if (diff.gameState) {
					that.memory.setCellStates(diff.gameState.memoryState);
					for (var i = 0; i < that.programs.length; ++i) {
						that.programs[i].setProgramState(diff.gameState.programStates[i]);
					}
				} else {
					that.memory.applyDiffs(diff.memoryDiffs);
					for (var i = 0; i < diff.programStateDiffs.length; ++i) {
						var programStateDiff = diff.programStateDiffs[i];
						that.programs[programStateDiff.program].applyDiff(programStateDiff);
					}
				}
			});
	}
});