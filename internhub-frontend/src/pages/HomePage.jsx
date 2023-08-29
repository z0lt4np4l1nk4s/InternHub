import React, { useState } from "react";
import { useEffect } from "react";
import { Loader } from "../components";
import { LoginService } from "../services";
import {
  AdminHomePage,
  CompanyHomePage,
  LandingPage,
  StudentHomePage,
  UserHomePage,
} from "./index";

export default function HomePage() {
  const loginService = new LoginService();
  const [loading, setLoading] = useState(true);
  const [role, setRole] = useState("");

  async function getUserRole() {
    let role = await loginService.getUserRoleAsync();
    role = role.toLowerCase();
    setRole(role);
    setLoading(false);
  }

  useEffect(() => {
    getUserRole();
  });

  if (loading) return <Loader />;

  if (role === "admin") return <AdminHomePage />;
  if (role === "company") return <CompanyHomePage />;
  if (role === "student") return <StudentHomePage />;
  if (role === "user") return <UserHomePage />;
  return <LandingPage />;
}
