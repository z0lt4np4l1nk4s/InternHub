import axios from "axios";
import {
  InternshipApplication,
  HttpHeader,
  PagedList,
  Server,
} from "../models";

const urlPrefix = Server.url + "InternshipApplication";

export class InternshipApplicationService {
  getUrlQuery({ pageNumber, pageSize, ...filter }) {
    const states =
      filter.states && filter.states.length > 0
        ? "&" + filter.states.map((state) => `states=${state}`).join("&")
        : "";
    const urlQuery = `?CurrentPage=${pageNumber}&pageSize=${pageSize}&companyId=${
      filter.companyId || ""
    }&studentId=${filter.studentId || ""}&companyName=${
      filter.companyName || ""
    }&firstName=${filter.firstName || ""}&lastName=${
      filter.lastName || ""
    }&internshipName=${filter.internshipName || ""}${states}`;
    return urlQuery;
  }

  async getAsync(params) {
    try {
      const response = await axios.get(urlPrefix + this.getUrlQuery(params), {
        headers: HttpHeader.get(),
      });
      if (response.status !== 200) return [];
      const dataList = response.data["Data"].map((data) =>
        InternshipApplication.fromJson(data)
      );
      console.log(dataList);
      const pagedList = PagedList.fromJson(response.data, dataList);
      return pagedList;
    } catch {
      return new PagedList({});
    }
  }

  async getUnacceptedAsync(params) {
    try {
      const response = await axios.get(
        urlPrefix + "/GetUnaccepted" + this.getUrlQuery(params),
        {
          headers: HttpHeader.get(),
        }
      );
      if (response.status !== 200) return [];
      const dataList = response.data["Data"].map((data) =>
        InternshipApplication.fromJson(data)
      );
      console.log(dataList);
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
      if (response.status !== 200) return null;
      return InternshipApplication.fromJson(response.data);
    } catch {
      return null;
    }
  }

  async postAsync(internshipId, applyMessage) {
    try {
      const response = await axios.post(
        urlPrefix,
        { InternshipId: internshipId, Message: applyMessage },
        {
          headers: HttpHeader.get(),
        }
      );
      return response.status === 200;
    } catch {
      return false;
    }
  }

  async acceptAsync(id, isAccepted) {
    try {
      const response = await axios.post(
        urlPrefix + `/Accept?id=${id}&isAccepted=${isAccepted}`,
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

  async deleteAsync(InternshipApplicationId) {
    try {
      const response = await axios.delete(
        urlPrefix + `/${InternshipApplicationId}`,
        { headers: HttpHeader.get() }
      );
      return response.status === 200;
    } catch {
      return false;
    }
  }

  async deleteByInternshipAsync(internshipId, studentId) {
    try {
      const response = await axios.delete(
        urlPrefix + `?internshipId=${internshipId}&studentId=${studentId}`,
        { headers: HttpHeader.get() }
      );
      return response.status === 200;
    } catch {
      return false;
    }
  }

  async getIdAsync(studentId, internshipId) {
    try {
      const response = await axios.get(
        urlPrefix +
          `/GetId?studentId=${studentId}&internshipId=${internshipId}`,
        { headers: HttpHeader.get() }
      );
      if (response.status === 200) {
        return response.data;
      }
      return null;
    } catch {
      return null;
    }
  }
}
