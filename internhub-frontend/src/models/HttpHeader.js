import { Server } from "./Server";

export class HttpHeader {
  static get() {
    return {
      Authorization: "Bearer " + Server.getToken(),
      "Content-Type": "application/json",
    };
  }
}
