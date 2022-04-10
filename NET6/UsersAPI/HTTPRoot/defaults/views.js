class View {
    constructor(Parent) {
        //this.myself    = Parent.appendChild(document.createElement('div')) as HTMLDivElement;
        //this.myself.id = Prefix + "View";
        //this.HeadlineDiv = Parent.appendChild(document.createElement('div')) as HTMLDivElement;
        //this.HeadlineDiv.id        = "headline";
        //this.HeadlineDiv.innerText = HeadlineText;
        this.ContentDiv = Parent;
        this.ContentDiv.innerText = "";
        //this.ButtonDiv   = Parent.appendChild(document.createElement('div')) as HTMLDivElement;
        //this.ButtonDiv.id          = "buttons";
        this.CurrentGroupDiv = this.ContentDiv;
    }
    CreateGroup(Prefix, Visible = true) {
        var GroupDiv = this.ContentDiv.appendChild(document.createElement('div'));
        GroupDiv.id = Prefix + "Group";
        GroupDiv.className = "rowGroup";
        if (!Visible)
            GroupDiv.style.display = "none";
        this.CurrentGroupDiv = GroupDiv;
        return GroupDiv;
    }
    ResetGroup() {
        this.CurrentGroupDiv = this.ContentDiv;
    }
    ;
    CreateRow(Prefix, KeyText, greeter = null) {
        return this.CreateRow2(Prefix, KeyText, false, greeter);
    }
    ;
    CreateRow2(Prefix, KeyText, keyTop = false, greeter = null) {
        var DivRow = this.CurrentGroupDiv.appendChild(document.createElement('div'));
        DivRow.id = Prefix + "Row";
        DivRow.className = "row";
        var DivKey = DivRow.appendChild(document.createElement('div'));
        DivKey.id = Prefix + "Key";
        DivKey.className = "key" + (keyTop ? " keyTop" : "");
        DivKey.innerText = KeyText;
        var DivValue = DivRow.appendChild(document.createElement('div'));
        DivValue.id = Prefix + "Value";
        DivValue.className = "value";
        if (greeter != null)
            greeter(DivValue);
        return DivValue;
    }
}
//# sourceMappingURL=views.js.map