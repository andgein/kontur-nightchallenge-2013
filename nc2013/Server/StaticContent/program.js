var ProgramState = Base.extend({
	constructor: function (options) {
		this.$processCount = options.$processCount;
		this.$next = options.$next;
		this.$win = options.$win;
		this.$source = options.$source;
		this.$enabled = options.$enabled;
		this.$startAddress = options.$startAddress;
		this.memory = options.memory;
		this.programIndex = options.programIndex;
		this.lastView = new CellView({
			$view: options.$last,
			useText: true
		});
		this.nextViews = [];
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
	draw: function () {
		this.$win.removeClass("winner");
		this.$win.addClass("draw");
	},
	autofocus: function (autofocus) {
		this.useAutofocus = autofocus;
	},
	nextProgram: function () {
		this._setNextCommand();
	},
	setProgramState: function (programState) {
		this._removeInstructionPointers();
		this.programState = programState;
		this._refreshState();
		this._setInstructionPointers();
	},
	setProgramStartInfo: function (programStartInfo) {
		this.$source.val(programStartInfo && programStartInfo.program || "");
		if (programStartInfo && programStartInfo.startAddress != null)
			this.$startAddress.val(programStartInfo.startAddress);
		else
			this.$startAddress.val("");
		if (!programStartInfo || !programStartInfo.disabled)
			this.$enabled.attr("checked", "checked");
		else
			this.$enabled.removeAttr("checked");
	},
	getProgramStartInfo: function () {
		var source = this.$source.val();
		var startAddress = this.$startAddress.val();
		var enabled = this.$enabled.is(":checked");
		return source && { program: source, startAddress: startAddress, disabled: !enabled };
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
			this.memory.getCell(this.programState.processPointers[i]).addInstructionPointer(this.programIndex);
	},
	_refreshState: function () {
		this.$processCount.text(this.programState ? this.programState.processPointers.length : "");

		if (this.programState && this.programState.lastPointer != null) {
			var lastCell = this.memory.getCell(this.programState.lastPointer);
			this.lastView.setCell(lastCell);
		} else
			this.lastView.setCell(null);
		if (this.programState) {
			for (var i = 0; i < this.nextViews.length; ++i) {
				if (i < this.programState.processPointers.length)
					this.nextViews[i].setCell(this.memory.getCell(this.programState.processPointers[i]));
				else
					this.nextViews[i].setCell(null);
			}
			if (this.nextViews.length < this.programState.processPointers.length) {
				var builder = new CellView.Builder({
					$container: this.$next,
					useText: true,
					cellClass: "listingItem"
				});
				for (var i = this.nextViews.length; i < this.programState.processPointers.length; ++i)
					builder.addCell(this.memory.getCell(this.programState.processPointers[i]));
				this.nextViews = this.nextViews.concat(builder.build());
			}
			if (!this._currentIsSet && this.nextViews.length > 0)
				this.nextViews[0].addClass("current");
		}
		else {
			for (var i = 0; i < this.nextViews.length; ++i)
				this.nextViews[i].setCell(null);
		}
	},
	_setNextCommand: function () {
		if (this.programState && this.programState.processPointers.length > 0) {
			var nextCell = this.memory.getCell(this.programState.processPointers[0]);
			nextCell.setNextCommand();
			if (this.useAutofocus)
				nextCell.scrollIntoView();
		}
	}
});