import React from "react";
import { useNavigate } from "react-router-dom";

export default function SmallLogo() {
  const navigate = useNavigate();
  return (
    <img
      src="/logo192.png"
      alt="logo"
      width={70}
      style={{
        backgroundColor: "transparent",
        cursor: "pointer",
        borderRadius: "40px",
      }}
      onClick={() => navigate("/")}
    />
  );
}
