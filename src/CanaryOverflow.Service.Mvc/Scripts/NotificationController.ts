import {Controller} from "@hotwired/stimulus";

class NotificationController extends Controller {
  static classes = ["hidden"];
  declare private readonly hiddenClass: string;
//todo: add has* and check existence

  static values = {
    isHidden: {type: Boolean, default: true}
  };
  declare private isHiddenValue: boolean;

  connect() {
    if (this.isHiddenValue) {
      this.element.classList.add(this.hiddenClass);
    }
  }

  isHiddenValueChanged(value: boolean) {
    if (value) {
      this.element.classList.add(this.hiddenClass);
    } else {
      this.element.classList.remove(this.hiddenClass);
    }
  }

  close() {
    this.isHiddenValue = true;
  }

  open() {
    this.isHiddenValue = false;
  }
}

export default NotificationController;
