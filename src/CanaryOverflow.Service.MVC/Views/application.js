import {Application} from "@hotwired/stimulus";
import NotificationController from "./Shared/notification_controller"

const stimulus = Application.start();
stimulus.register("notification", NotificationController);
