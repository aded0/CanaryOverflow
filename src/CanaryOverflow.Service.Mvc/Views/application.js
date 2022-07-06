import {Application} from "@hotwired/stimulus";
import NotificationController from "./Shared/notification_controller"
import * as Turbo from "@hotwired/turbo"

const stimulus = Application.start();
stimulus.register("notification", NotificationController);

Turbo.start();
