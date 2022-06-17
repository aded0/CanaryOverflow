import {Controller} from "@hotwired/stimulus";

export default class extends Controller {
  static classes = ["hidden"];
  static values = {
    isHidden: {type: Boolean, default: true}
  };

  connect() {
    if (this.isHiddenValue) {
      this.element.classList.add(this.hiddenClass);
    }
  }

  isHiddenValueChanged(value) {
    if (value) {
      this.element.classList.add(this.hiddenClass);
    } else {
      this.element.classList.remove(this.hiddenClass);
    }
  }

  close() {
    this.isHiddenValue = true;
  }
}
