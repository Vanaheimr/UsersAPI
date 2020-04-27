///<reference path="../libs/date.format.ts" />
var DatePicker = /** @class */ (function () {
    function DatePicker() {
        this.show = this.show.bind(this);
        this.paint = this.paint.bind(this);
        this.hide = this.hide.bind(this);
    }
    DatePicker.prototype.show = function (InputDiv, StartDateUTC, resultDelegate) {
        var _this = this;
        this.resultDelegate = resultDelegate;
        moment.locale(window.navigator.language);
        this.selectedDate = StartDateUTC != null && StartDateUTC !== "" ? moment.utc(StartDateUTC).local() : moment().local();
        this.currentMonth = moment(this.selectedDate.format('YYYY-MM'), "YYYY-MM");
        this.frameDiv = InputDiv.parentElement.appendChild(document.createElement('div'));
        this.frameDiv.className = "calendarPicker";
        var topDiv = this.frameDiv.appendChild(document.createElement('div'));
        topDiv.className = "top";
        var leftDiv = topDiv.appendChild(document.createElement('div'));
        leftDiv.className = "left";
        var leftButton = leftDiv.appendChild(document.createElement('button'));
        leftButton.innerHTML = "<i class=\"fas fa-angle-left\"></i>";
        leftButton.onclick = function () {
            _this.currentMonth = _this.currentMonth.subtract(1, 'months');
            _this.paint();
        };
        this.centerDiv = topDiv.appendChild(document.createElement('div'));
        this.centerDiv.className = "center";
        var rightDiv = topDiv.appendChild(document.createElement('div'));
        rightDiv.className = "right";
        var rightButton = rightDiv.appendChild(document.createElement('button'));
        rightButton.innerHTML = "<i class=\"fas fa-angle-right\"></i>";
        rightButton.onclick = function () {
            _this.currentMonth = _this.currentMonth.add(1, 'months');
            _this.paint();
        };
        this.daysDiv = this.frameDiv.appendChild(document.createElement('div'));
        this.daysDiv.className = "days";
        var removeDateDiv = this.frameDiv.appendChild(document.createElement('div'));
        removeDateDiv.className = "removeDate";
        var removeDateButton = removeDateDiv.appendChild(document.createElement('button'));
        removeDateButton.innerHTML = "<i class=\"fas fa-trash-alt\"></i> remove date";
        removeDateButton.onclick = function () {
            _this.selectedDate = null;
            document.removeEventListener('click', _this.hide, true);
            _this.frameDiv.remove();
            resultDelegate("");
        };
        this.paint();
        document.addEventListener('click', this.hide, true);
    };
    DatePicker.prototype.paint = function () {
        var _this = this;
        this.centerDiv.innerHTML = this.currentMonth.format('MMMM YYYY');
        this.daysDiv.innerHTML = "";
        for (var dayOfWeek = 0; dayOfWeek <= 6; dayOfWeek++) {
            var dayOfWeekDiv = this.daysDiv.appendChild(document.createElement('div'));
            dayOfWeekDiv.className = "dayOfWeek";
            dayOfWeekDiv.innerHTML = moment.weekdaysShort(dayOfWeek).replace('.', '');
        }
        for (var day = 0; day < parseInt(this.currentMonth.format("d")); day++)
            this.daysDiv.appendChild(document.createElement('div'));
        var _loop_1 = function (day) {
            var currentDay = moment(this_1.currentMonth.format('YYYY-MM')).add(day - 1, 'days');
            var dayDiv = this_1.daysDiv.appendChild(document.createElement('div'));
            dayDiv.id = "day_" + currentDay.format('YYYY-MM-DD');
            dayDiv.className = "day";
            dayDiv.innerHTML = day.toString();
            dayDiv.onclick = function () {
                var currentDateDiv = document.getElementById("day_" + _this.selectedDate.format('YYYY-MM-DD'));
                if (currentDateDiv != null)
                    currentDateDiv.style.backgroundColor = null;
                dayDiv.style.backgroundColor = "#FF0000";
                _this.selectedDate = currentDay;
                _this.resultDelegate(_this.selectedDate.toISOString());
            };
            dayDiv.ondblclick = function () {
                var currentDateDiv = document.getElementById("day_" + _this.selectedDate.format('YYYY-MM-DD'));
                if (currentDateDiv != null)
                    currentDateDiv.style.backgroundColor = null;
                dayDiv.style.backgroundColor = "#FF0000";
                _this.selectedDate = currentDay;
                document.removeEventListener('click', _this.hide, true);
                _this.frameDiv.remove();
                _this.resultDelegate(_this.selectedDate.toISOString());
            };
            if (this_1.selectedDate.format('YYYY-MM-DD') === currentDay.format('YYYY-MM-DD'))
                dayDiv.style.backgroundColor = "#FF0000";
        };
        var this_1 = this;
        for (var day = 1; day <= this.currentMonth.daysInMonth(); day++) {
            _loop_1(day);
        }
    };
    DatePicker.prototype.hide = function (ev) {
        try {
            var clickedDiv = ev.target;
            do {
                if (clickedDiv === this.frameDiv)
                    return;
                clickedDiv = clickedDiv.parentElement;
            } while (clickedDiv != null);
            this.frameDiv.remove();
            document.removeEventListener('click', this.hide, true);
        }
        catch (exception) { }
    };
    return DatePicker;
}());
//# sourceMappingURL=datePicker.js.map