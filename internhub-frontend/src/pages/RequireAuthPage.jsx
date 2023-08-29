import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import { LoginService } from "../services";
import { NotFoundPage } from "../pages";
import { Loader } from "../components";

export default function RequireAuthPage({ roles, page }) {
  const navigate = useNavigate();
  const [role, setRole] = useState("");
  const [loading, setLoading] = useState(true);
  const loginService = new LoginService();

  async function getUserRole() {
    let role = await loginService.getUserRoleAsync();
    role = role.toLowerCase();
    setRole(role);
    setLoading(false);
  }

  useEffect(() => {
    if (!loginService.isUserTokenValid()) {
      navigate("/login");
    } else getUserRole();
  }, []);

  if (loading) return <Loader />;

  if (
    !roles ||
    roles.length === 0 ||
    roles.map((role) => role.toLowerCase()).includes(role)
  ) {
    return page;
  }

  return <NotFoundPage />;
}
