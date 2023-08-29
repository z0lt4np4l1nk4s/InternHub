import axios from "axios";
import { HttpHeader, Server, State } from "../models";

const urlPrefix = Server.url + "State";

export class StateService {
  async getAsync() {
    try {
      const response = await axios.get(urlPrefix, {
        headers: HttpHeader.get(),
      });
      if (response.status !== 200) return [];
      return response.data.map((data) => State.fromJson(data));
    } catch {
      return [];
    }
  }
}
