import axios from "axios";
import { Company, HttpHeader, PagedList, Server } from "../models";

const urlPrefix = Server.url + "Company";

export class CompanyService {
  async getAsync({ pageNumber, pageSize, ...filter }) {
    try {
      const response = await axios.get(
        urlPrefix +
          `?CurrentPage=${pageNumber}&pageSize=${pageSize}&isAccepted=${
            filter.isAccepted
          }&name=${filter.name || ""}&isActive=${filter.isActive}`,
        {
          headers: HttpHeader.get(),
        }
      );
      if (response.status !== 200) return [];
      const dataList = response.data["Data"].map((data) =>
        Company.fromJson(data)
      );
      const pagedList = PagedList.fromJson(response.data, dataList);
      return pagedList;
    } catch {
      return new PagedList({});
    }
  }

  async getByIdAsync(id) {
    try {
      const response = await axios.get(urlPrefix + "?id=" + id, {
        headers: HttpHeader.get(),
      });
      console.log("API response:", response.data);
      if (response.status !== 200) return null;
      return Company.fromJson(response.data);
    } catch {
      return null;
    }
  }

  async postAsync(company) {
    try {
      const response = await axios.post(urlPrefix, company, {
        headers: HttpHeader.get(),
      });
      return response.status === 200;
    } catch {
      return false;
    }
  }

  async updateAsync(id, company) {
    try {
      const response = await axios.put(urlPrefix + "/" + id, company, {
        headers: HttpHeader.get(),
      });
      return response.status === 200;
    } catch {
      return false;
    }
  }

  async removeAsync(id) {
    try {
      const response = await axios.delete(urlPrefix + "/" + id, {
        headers: HttpHeader.get(),
      });
      return response.status === 200;
    } catch {
      return false;
    }
  }

  async approveAsync(id, isAccepted) {
    try {
      const response = await axios.put(
        urlPrefix + `/Approve?id=${id}&isAccepted=${isAccepted}`,
        null,
        {
          headers: HttpHeader.get(),
        }
      );
      return response.status === 200;
    } catch {
      return false;
    }
  }
}
