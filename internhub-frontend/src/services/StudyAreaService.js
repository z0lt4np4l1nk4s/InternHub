import axios from "axios";
import { HttpHeader, Server, StudyArea } from "../models";

const urlPrefix = Server.url + "StudyArea";

export class StudyAreaService {
  async getAsync() {
    try {
      const response = await axios.get(urlPrefix, {
        headers: HttpHeader.get(),
      });
      if (response.status !== 200) return [];
      return response.data.map((data) => StudyArea.fromJson(data));
    } catch {
      return [];
    }
  }
}
