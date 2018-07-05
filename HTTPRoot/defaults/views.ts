

interface DivElementAction {
    (DoWith: HTMLDivElement): void;
}

class View {

    myself:           HTMLDivElement;
    HeadlineDiv:      HTMLDivElement;
    ContentDiv:       HTMLDivElement;
    CurrentGroupDiv:  HTMLDivElement;
    ButtonDiv:        HTMLDivElement;

    constructor(Parent:        HTMLDivElement,
                Prefix:        string,
                HeadlineText:  string) {

        this.myself = Parent.appendChild(document.createElement('div')) as HTMLDivElement;
        this.myself.id = Prefix + "View";

        this.HeadlineDiv = this.myself.appendChild(document.createElement('div')) as HTMLDivElement;
        this.HeadlineDiv.id        = "headline";
        this.HeadlineDiv.innerText = HeadlineText;

        this.ContentDiv  = this.myself.appendChild(document.createElement('div')) as HTMLDivElement;
        this.ContentDiv.id         = "content";

        this.ButtonDiv   = this.myself.appendChild(document.createElement('div')) as HTMLDivElement;
        this.ButtonDiv.id          = "buttons";

        this.CurrentGroupDiv = this.ContentDiv;

    }

    CreateGroup(Prefix:  string,
                Visible: boolean = true) {

        var GroupDiv = this.ContentDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        GroupDiv.id = Prefix + "Group";
        GroupDiv.className = "rowGroup";

        if (!Visible)
            GroupDiv.style.display = "none";

        this.CurrentGroupDiv = GroupDiv;
        return GroupDiv;

    }

    CreateRow(Prefix: string,
        KeyText: string,
        greeter: DivElementAction = null) {

        return this.CreateRow2(
            Prefix,
            KeyText,
            false,
            greeter);

    };

    CreateRow2(
        Prefix: string,
        KeyText: string,
        keyTop: boolean = false,
        greeter: DivElementAction = null) {


        var DivRow = this.CurrentGroupDiv.appendChild(document.createElement('div')) as HTMLDivElement;
        DivRow.id = Prefix + "Row";
        DivRow.className = "row";

        var DivKey = DivRow.appendChild(document.createElement('div')) as HTMLDivElement;
        DivKey.id = Prefix + "Key";
        DivKey.className = "key" + (keyTop ? " keyTop" : "");
        DivKey.innerText = KeyText;

        var DivValue = DivRow.appendChild(document.createElement('div')) as HTMLDivElement;
        DivValue.id = Prefix + "Value";
        DivValue.className = "value";

        if (greeter != null)
            greeter(DivValue);

        return DivValue;

    }

    CreateButton(Prefix: string,
                 Name:   string) {

        var NewButton = this.ButtonDiv.appendChild(document.createElement('button')) as HTMLButtonElement;
        NewButton.id = Prefix + "Button";
        NewButton.className = "button";
        NewButton.innerHTML = Name;

        return NewButton;

    }

    CancelButton(Prefix: string,
                 Name:   string) {

        var me = this;

        var CancelButton = this.CreateButton(Prefix, Name);

        CancelButton.onclick = function (this: HTMLElement, ev: MouseEvent) {
            me.myself.remove();
        };

        return CancelButton;

    }


}
