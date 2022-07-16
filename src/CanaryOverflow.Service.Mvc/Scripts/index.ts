import * as Turbo from '@hotwired/turbo'
import {Application} from '@hotwired/stimulus';
import NotificationController from './NotificationController'

Turbo.start();
const application = Application.start();
application.register('notification', NotificationController);
