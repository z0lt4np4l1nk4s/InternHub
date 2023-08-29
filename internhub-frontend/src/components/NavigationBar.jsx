import React from "react";
import {
  AdminNavigation,
  CompanyNavigation,
  StudentNavigation,
  UserNavigation,
} from "./index";
import { LoginService } from "../services";

export default function NavigationBar() {
  const userToken = new LoginService().getUserToken() ?? { role: "" };
  const role = userToken.role.toLowerCase();
  if (role === "admin") return <AdminNavigation />;
  if (role === "student") return <StudentNavigation />;
  if (role === "company") return <CompanyNavigation />;
  return <UserNavigation />;
}
