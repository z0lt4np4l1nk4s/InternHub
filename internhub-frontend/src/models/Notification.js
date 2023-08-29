import { User } from "./User";

export class Notification {
  constructor({ title = "", body = "", user = null }) {
    this.title = title;
    this.body = body;
    this.user = user;
  }

  static fromJson(json) {
    return new Notification({
      title: json["Title"],
      body: json["Body"],
      user: json["User"] ? User.fromJson(json["User"]) : null,
    });
  }
}
