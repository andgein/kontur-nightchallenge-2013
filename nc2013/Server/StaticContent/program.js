var ProgramState = Base.extend({
	constructor: function (containers, memory, programStartInfo) {
		this.$taskCount = containers.$taskCount;
		this.$last = containers.$last;
		this.$next = containers.$next;
		this.programStartInfo = programStartInfo;
		this.memory = memory;
	},
	applyDiff: function (programStateDiff) {
		switch (programStateDiff.changeType) {
			case "executed":
				this.programState.lastPointer = this.programState.processPointers.shift();
				this.programState.processPointers.push(programStateDiff.nextPointer);
				break;
			case "killed":
				this.programState.lastPointer = this.programState.processPointers.shift();
				break;
			case "splitted":
				this.programState.processPointers.push(programStateDiff.nextPointer);
				break;
			default:
				throw "Invalid changeType: " + programStateDiff.changeType;
		}
		this._refreshState();
	},
	setProgramState: function (programState) {
		this.programState = programState;
		this._refreshState();
	},
	_refreshState: function () {
		this.$taskCount.text(this.programState.processPointers.length);
		if (this.programState.lastPointer) {
			var lastCell = this.memory.getCell(this.programState.lastPointer);
			this.$last.text(lastCell.getListingItemContent());
		}
		if (this.programState.processPointers.length > 0) {
			var nextCell = this.memory.getCell(this.programState.processPointers[0]);
			this.$next.text(nextCell.getListingItemContent());
		}
	}
});