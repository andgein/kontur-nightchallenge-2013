var Cell = Base.extend({
	constructor: function (options) {
		this.address = options.address;
		this.memoryData = options.memoryData;
		this.updateCounter = 0;
		this.views = [];
		this._setInitialState();
	},
	_setInitialState: function () {
		this._text = "";
		this._cellType = "Data";
		this._lastModifiedByProgram = null;
		this._isNextCommand = false;
		this._isJustScrolled = false;
		this._activeInstructionPointers = {};
	},
	setCellState: function (cellState) {
		this.beginUpdate();
		if (!cellState) {
			this._newText = "";
			this._newCellType = "Data";
			this._newLastModifiedByProgram = null;
		} else {
			this._newText = this.address + " " + cellState.instruction;
			this._newCellType = cellState.cellType;
			this._newLastModifiedByProgram = cellState.lastModifiedByProgram;
		}
		this.endUpdate();
	},
	setNextCommand: function () {
		this._setIsNextCommand(true);
	},
	_setIsNextCommand: function (isNextCommand) {
		this.beginUpdate();
		this._newIsNextCommand = isNextCommand;
		this.endUpdate();
	},
	scrollIntoView: function () {
		this._setIsJustScrolled(true);
	},
	_setIsJustScrolled: function (justScrolled) {
		this.beginUpdate();
		this._newIsJustScrolled = justScrolled;
		this.endUpdate();
	},
	reset: function () {
		this.beginUpdate();
		this._newText = "";
		this._newCellType = "Data";
		this._newLastModifiedByProgram = null;
		this._newIsNextCommand = false;
		this._newIsJustScrolled = false;
		this._newInstructionPointers = {};
		for (var programIndex in this._activeInstructionPointers)
			this._newInstructionPointers[programIndex] = false;
		this.endUpdate();
	},
	addInstructionPointer: function (programIndex) {
		this.beginUpdate();
		if (!this._newInstructionPointers)
			this._newInstructionPointers = {};
		this._newInstructionPointers[programIndex] = true;
		this.endUpdate();
	},
	removeInstructionPointer: function (programIndex) {
		this.beginUpdate();
		if (!this._newInstructionPointers)
			this._newInstructionPointers = {};
		this._newInstructionPointers[programIndex] = false;
		this.endUpdate();
	},
	beginUpdate: function () {
		this.updateCounter++;
	},
	endUpdate: function () {
		this.updateCounter--;
		if (this.updateCounter < 0)
			throw "Invalid call to endUpdate";
		if (this.updateCounter == 0) {
			if (this._newText !== undefined && this._text != this._newText) {
				this._text = this._newText;
				this._setViewText(this._text || "");
			}
			if (this._newCellType !== undefined && this._cellType != this._newCellType) {
				this._removeViewClass(this._cellType);
				this._cellType = this._newCellType;
				this._addViewClass(this._cellType);
			}
			if (this._newLastModifiedByProgram !== undefined && this._lastModifiedByProgram != this._newLastModifiedByProgram) {
				if (this._lastModifiedByProgram != null)
					this._removeViewClass("modified" + this._lastModifiedByProgram);
				this._lastModifiedByProgram = this._newLastModifiedByProgram;
				if (this._lastModifiedByProgram != null)
					this._addViewClass("modified" + this._lastModifiedByProgram);
			}
			if (this._newIsJustScrolled !== undefined && this._isJustScrolled != this._newIsJustScrolled) {
				if (this._isJustScrolled) {
					this._removeViewClass("justScrolled");
					this.memoryData.justScrolledCell = null;
				}
				this._isJustScrolled = this._newIsJustScrolled;
				if (this._isJustScrolled) {
					if (this.memoryData.justScrolledCell)
						this.memoryData.justScrolledCell._setIsJustScrolled(false);
					this._addViewClass("justScrolled");
					this._scrollToView();
					this.memoryData.justScrolledCell = this;
				}
			}
			if (this._newIsNextCommand !== undefined && this._isNextCommand != this._newIsNextCommand) {
				if (this._isNextCommand) {
					this._removeViewClass("nextCommand");
					this.memoryData.nextCommandCell = null;
				}
				this._isNextCommand = this._newIsNextCommand;
				if (this._isNextCommand) {
					if (this.memoryData.nextCommandCell)
						this.memoryData.nextCommandCell._setIsNextCommand(false);
					this._addViewClass("nextCommand");
					this.memoryData.nextCommandCell = this;
				}
			}
			if (this._newInstructionPointers !== undefined)
				for (var programIndex in this._newInstructionPointers) {
					if (this._newInstructionPointers[programIndex] && !this._activeInstructionPointers[programIndex]) {
						this._addViewClass("ip" + programIndex);
						this._activeInstructionPointers[programIndex] = true;
					}
					else if (!this._newInstructionPointers[programIndex] && this._activeInstructionPointers[programIndex]) {
						this._removeViewClass("ip" + programIndex);
						delete this._activeInstructionPointers[programIndex];
					}
				}
			delete this._newText;
			delete this._newCellType;
			delete this._newLastModifiedByProgram;
			delete this._newIsJustScrolled;
			delete this._newIsNextCommand;
		}
	},
	attachView: function (view) {
		this.views.push(view);
		if (this._text)
			view.setText(this._text);
		view.addClass(this._cellType);
		if (this._isJustScrolled)
			view.addClass("justScrolled");
		if (this._isNextCommand)
			view.addClass("nextCommand");
		if (this._lastModifiedByProgram != null)
			view.addClass("modified" + this._lastModifiedByProgram);
		for (var programIndex in this._activeInstructionPointers)
			view.addClass("ip" + programIndex);
	},
	attachReadyView: function (view) {
		this.views.push(view);
	},
	getViewClass: function () {
		var viewClass = "";
		viewClass += this._cellType;
		if (this._isJustScrolled)
			viewClass += " justScrolled";
		if (this._isNextCommand)
			viewClass += " nextCommand";
		if (this._lastModifiedByProgram != null)
			viewClass += " modified" + this._lastModifiedByProgram;
		for (var programIndex in this._activeInstructionPointers)
			viewClass += " ip" + programIndex;
		return viewClass;
	},
	getViewText: function () {
		return this._text;
	},
	detachView: function (view) {
		for (var i = 0; i < this.views.length; i++) {
			if (this.views[i] === view) {
				this.views.splice(i, 1);
				if (this._text)
					view.setText("");
				view.removeClass(this._cellType);
				if (this._isJustScrolled)
					view.removeClass("justScrolled");
				if (this._isNextCommand)
					view.removeClass("nextCommand");
				if (this._lastModifiedByProgram != null)
					view.removeClass("modified" + this._lastModifiedByProgram);
				for (var programIndex in this._activeInstructionPointers)
					view.removeClass("ip" + programIndex);
				return;
			}
		}
	},
	_setViewText: function (text) {
		for (var i = 0; i < this.views.length; ++i)
			this.views[i].setText(text);
	},
	_addViewClass: function (viewClass) {
		for (var i = 0; i < this.views.length; ++i)
			this.views[i].addClass(viewClass);
	},
	_removeViewClass: function (viewClass) {
		for (var i = 0; i < this.views.length; ++i)
			this.views[i].removeClass(viewClass);
	},
	_scrollToView: function () {
		for (var i = 0; i < this.views.length; ++i)
			this.views[i].scrollIntoView();
	}
});