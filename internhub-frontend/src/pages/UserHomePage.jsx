import React from "react";
import { NavigationBar } from "../components";

export default function UserHomePage() {
  return (
    <div>
      <NavigationBar />
      <div
        className="d-flex justify-content-center align-items-center"
        style={{ "min-height": "100vh" }}
      >
        <div className="row">
          <div className="col text-center">
            <img src="/images/pending_approval.svg" alt="Pending approval" />
            <div style={{ margin: "30px", "margin-top": "60px" }}>
              <h1>Your account is under approval!</h1>
            </div>
            <h4>Our administrators will look at your profile</h4>
            <h4>and decide whether you fit our requirements.</h4>
            <h4>We will contact you shortly!</h4>
          </div>
        </div>
      </div>
    </div>
  );
}
