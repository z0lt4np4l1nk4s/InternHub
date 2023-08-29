import axios from "axios";
import { County, HttpHeader, Server } from "../models";

const urlPrefix = Server.url + "County";

export class CountyService {
  async getAsync() {
    try {
      const response = await axios.get(urlPrefix, {
        headers: HttpHeader.get(),
      });
      if (response.status !== 200) return [];
      return response.data.map((data) => County.fromJson(data));
    } catch {
      return [];
    }
  }
}
