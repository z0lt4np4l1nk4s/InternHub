import { LoginService } from "../services/index";

export class Server {
  static host = "https://localhost:44332/";
  static url = "https://localhost:44332/api/";
  static getToken = () => {
    const userToken = new LoginService().getUserToken();
    if (!userToken) return "";
    return userToken.token;
  };
}
