import React from "react";

export default function Loader() {
  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div
        className="spinner-border"
        style={{ width: "3rem", height: "3rem" }}
        role="status"
      ></div>
    </div>
  );
}
