import React from "react";
import { NavigationBar } from "../components";

export default function NotFoundPage() {
  return (
    <>
      <NavigationBar />
      <div className="vh-100 vw-100 d-flex align-items-center justify-content-center">
        <div>
          <div className="row">
            <div className="col text-center">
              <h1>Looks like nothing has been found!</h1>
            </div>
          </div>
          <div className="row" style={{ height: 80 }}></div>
          <div className="row">
            <div className="col d-flex justify-content-center">
              <img src="/images/not_found.svg" alt="Not Found" width={700} />
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
