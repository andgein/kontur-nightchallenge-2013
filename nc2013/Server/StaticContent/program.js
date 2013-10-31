var ProgramState = Base.extend({
	constructor: function (options) {
		this.$processCount = options.$processCount;
		this.$last = options.$last;
		this.$next = options.$next;
		this.$win = options.$win;
		this.$source = options.$source;
		this.memory = options.memory;
		this.programIndex = options.programIndex;
	},
	applyDiff: function (programStateDiff) {
		this._removeInstructionPointers();
		switch (programStateDiff.changeType) {
			case "Executed":
				this.programState.lastPointer = this.programState.processPointers.shift();
				this.programState.processPointers.push(programStateDiff.nextPointer);
				break;
			case "Killed":
				this.programState.lastPointer = this.programState.processPointers.shift();
				break;
			case "Splitted":
				this.programState.processPointers.push(programStateDiff.nextPointer);
				break;
			default:
				throw "Invalid changeType: " + programStateDiff.changeType;
		}
		this._refreshState();
		this._setInstructionPointers();
	},
	reset: function () {
		this.setProgramState(null);
		this.$win.removeClass("winner");
		this.$win.removeClass("draw");
		this.$win.removeClass("current");
	},
	plays: function () {
		this.$win.removeClass("draw");
		this.$win.removeClass("winner");
	},
	win: function () {
		this.$win.removeClass("draw");
		this.$win.addClass("winner");
	},
	draw: function() {
		this.$win.removeClass("winner");
		this.$win.addClass("draw");
	},
	current: function (isCurrent) {
		if (isCurrent)
			this.$next.addClass("current");
		else
			this.$next.removeClass("current");
	},
	setProgramState: function (programState) {
		this._removeInstructionPointers();
		this.programState = programState;
		this._refreshState();
		this._setInstructionPointers();
	},
	setProgramStartInfo: function (programStartInfo) {
		this.$source.val(programStartInfo && programStartInfo.program || "");
	},
	getProgramStartInfo: function () {
		var source = this.$source.val();
		return source && { program: source };
	},
	_removeInstructionPointers: function () {
		if (!this.programState || !this.programState.processPointers)
			return;
		for (var i = 0; i < this.programState.processPointers.length; ++i)
			this.memory.getCell(this.programState.processPointers[i]).removeInstructionPointer(this.programIndex);
	},
	_setInstructionPointers: function () {
		if (!this.programState || !this.programState.processPointers)
			return;
		for (var i = 0; i < this.programState.processPointers.length; ++i)
			this.memory.getCell(this.programState.processPointers[i]).setInstructionPointer(this.programIndex);
	},
	_refreshState: function () {
		this.$processCount.text(this.programState ? this.programState.processPointers.length : "");

		if (this.programState && this.programState.lastPointer) {
			var lastCell = this.memory.getCell(this.programState.lastPointer);
			this.$last.text(lastCell.getListingItemContent());
		} else
			this.$last.text("");

		if (this.programState && this.programState.processPointers.length > 0) {
			var nextCell = this.memory.getCell(this.programState.processPointers[0]);
			this.$next.text(nextCell.getListingItemContent());
		}
		else
			this.$next.text("");
	}
});