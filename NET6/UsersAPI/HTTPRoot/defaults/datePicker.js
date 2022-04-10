///<reference path="../libs/date.format.ts" />
class DatePicker {
    constructor() {
        this.show = this.show.bind(this);
        this.paint = this.paint.bind(this);
        this.hide = this.hide.bind(this);
    }
    show(InputDiv, StartDateUTC, resultDelegate) {
        this.resultDelegate = resultDelegate;
        moment.locale(window.navigator.language);
        this.selectedDate = StartDateUTC != null && StartDateUTC !== "" ? moment.utc(StartDateUTC).local() : moment().local();
        this.currentMonth = moment(this.selectedDate.format('YYYY-MM'), "YYYY-MM");
        this.frameDiv = InputDiv.parentElement.appendChild(document.createElement('div'));
        this.frameDiv.className = "calendarPicker";
        const topDiv = this.frameDiv.appendChild(document.createElement('div'));
        topDiv.className = "top";
        const leftDiv = topDiv.appendChild(document.createElement('div'));
        leftDiv.className = "left";
        const leftButton = leftDiv.appendChild(document.createElement('button'));
        leftButton.id = "calendarPickerLeftButton";
        leftButton.innerHTML = "<i class=\"fas fa-angle-left\"></i>";
        leftButton.onclick = () => {
            this.currentMonth = this.currentMonth.subtract(1, 'months');
            this.paint();
        };
        this.centerDiv = topDiv.appendChild(document.createElement('div'));
        this.centerDiv.className = "center";
        const rightDiv = topDiv.appendChild(document.createElement('div'));
        rightDiv.className = "right";
        const rightButton = rightDiv.appendChild(document.createElement('button'));
        rightButton.id = "calendarPickerRightButton";
        rightButton.innerHTML = "<i id=\"rightArrow\" class=\"fas fa-angle-right\"></i>";
        rightButton.onclick = () => {
            this.currentMonth = this.currentMonth.add(1, 'months');
            this.paint();
        };
        this.daysDiv = this.frameDiv.appendChild(document.createElement('div'));
        this.daysDiv.className = "days";
        const removeDateDiv = this.frameDiv.appendChild(document.createElement('div'));
        removeDateDiv.className = "removeDate";
        const removeDateButton = removeDateDiv.appendChild(document.createElement('button'));
        removeDateButton.id = "removeDateButton";
        removeDateButton.innerHTML = "<i id=\"trashIcon\" class=\"fas fa-trash-alt\"></i> remove date";
        removeDateButton.onclick = () => {
            this.selectedDate = null;
            document.removeEventListener('click', this.hide, true);
            this.frameDiv.remove();
            resultDelegate("");
        };
        this.paint();
        document.addEventListener('click', this.hide, true);
    }
    paint() {
        this.centerDiv.innerHTML = this.currentMonth.format('MMMM YYYY');
        this.daysDiv.innerHTML = "";
        for (let dayOfWeek = 0; dayOfWeek <= 6; dayOfWeek++) {
            const dayOfWeekDiv = this.daysDiv.appendChild(document.createElement('div'));
            dayOfWeekDiv.className = "dayOfWeek";
            dayOfWeekDiv.innerHTML = moment.weekdaysShort(dayOfWeek).replace('.', '');
        }
        for (let day = 0; day < parseInt(this.currentMonth.format("d")); day++)
            this.daysDiv.appendChild(document.createElement('div'));
        for (let day = 1; day <= this.currentMonth.daysInMonth(); day++) {
            const currentDay = moment(this.currentMonth.format('YYYY-MM')).add(day - 1, 'days');
            const dayDiv = this.daysDiv.appendChild(document.createElement('div'));
            dayDiv.id = "day_" + currentDay.format('YYYY-MM-DD');
            dayDiv.className = "day";
            dayDiv.innerHTML = day.toString();
            dayDiv.onclick = () => {
                const currentDateDiv = document.getElementById("day_" + this.selectedDate.format('YYYY-MM-DD'));
                if (currentDateDiv != null)
                    currentDateDiv.style.backgroundColor = null;
                dayDiv.style.backgroundColor = "#FF0000";
                this.selectedDate = currentDay;
                this.resultDelegate(this.selectedDate.toISOString());
            };
            dayDiv.ondblclick = () => {
                const currentDateDiv = document.getElementById("day_" + this.selectedDate.format('YYYY-MM-DD'));
                if (currentDateDiv != null)
                    currentDateDiv.style.backgroundColor = null;
                dayDiv.style.backgroundColor = "#FF0000";
                this.selectedDate = currentDay;
                document.removeEventListener('click', this.hide, true);
                this.frameDiv.remove();
                this.resultDelegate(this.selectedDate.toISOString());
            };
            if (this.selectedDate.format('YYYY-MM-DD') === currentDay.format('YYYY-MM-DD'))
                dayDiv.style.backgroundColor = "#FF0000";
        }
    }
    hide(ev) {
        try {
            let clickedDiv = ev.target;
            do {
                if (clickedDiv === this.frameDiv)
                    return;
                clickedDiv = clickedDiv.parentElement;
            } while (clickedDiv != null);
            this.frameDiv.remove();
            document.removeEventListener('click', this.hide, true);
        }
        catch (exception) { }
    }
}
//# sourceMappingURL=datePicker.js.map