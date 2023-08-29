import axios from "axios";
import { HttpHeader, Notification, PagedList, Server } from "../models";

const urlPrefix = Server.url + "Notification";

export class NotificationService {
  async getAsync(pageNumber) {
    try {
      const response = await axios.get(
        urlPrefix + `?CurrentPage=${pageNumber}&pageSize=5`,
        {
          headers: HttpHeader.get(),
        }
      );
      if (response.status !== 200) return [];
      const dataList = response.data["Data"].map((data) =>
        Notification.fromJson(data)
      );
      const pagedList = PagedList.fromJson(response.data, dataList);
      return pagedList;
    } catch {
      return new PagedList({});
    }
  }
}
