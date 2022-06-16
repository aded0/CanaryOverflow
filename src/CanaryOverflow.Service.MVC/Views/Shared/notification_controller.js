import {Controller} from "@hotwired/stimulus";

export default class extends Controller {
  close() {
    console.log("notification closed")
  }
}
