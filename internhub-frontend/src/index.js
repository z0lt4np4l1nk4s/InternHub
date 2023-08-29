import React from "react";
import ReactDOM from "react-dom/client";
import "./styles/index.css";
import reportWebVitals from "./reportWebVitals";
import { createBrowserRouter, RouterProvider } from "react-router-dom";
import {
  AdminCompaniesPage,
  AdminStudentsPage,
  HomePage,
  CompanyRegisterPage,
  LoginPage,
  StudentDetailsPage,
  StudentEditPage,
  InternshipCreate,
  StudentRegisterPage,
  CompanyProfilePage,
  CompanyEditPage,
  RequireAuthPage,
  StudentInternships,
  CompanyInternshipApplications,
  InternshipDetails,
  InternshipEdit,
} from "./pages";

const router = createBrowserRouter([
  {
    path: "/",
    element: <HomePage />,
  },
  {
    path: "/login",
    element: <LoginPage />,
  },
  {
    path: "/students",
    element: <RequireAuthPage roles={["Admin"]} page={<AdminStudentsPage />} />,
  },
  {
    path: "/companies",
    element: (
      <RequireAuthPage roles={["Admin"]} page={<AdminCompaniesPage />} />
    ),
  },
  {
    path: "/student/register",
    element: <StudentRegisterPage />,
  },
  {
    path: "/student/edit/:id",
    element: (
      <RequireAuthPage
        roles={["Student", "Admin"]}
        page={<StudentEditPage />}
      />
    ),
  },
  {
    path: "/student/details/:id",
    element: <RequireAuthPage roles={[]} page={<StudentDetailsPage />} />,
  },
  {
    path: "/student/internships",
    element: (
      <RequireAuthPage roles={["Student"]} page={<StudentInternships />} />
    ),
  },
  {
    path: "/company/register",
    element: <CompanyRegisterPage />,
  },
  {
    path: "/company/details/:id",
    element: <RequireAuthPage roles={[]} page={<CompanyProfilePage />} />,
  },
  {
    path: "/company/edit/:id",
    element: (
      <RequireAuthPage
        roles={["Company", "Admin"]}
        page={<CompanyEditPage />}
      />
    ),
  },
  {
    path: "/internship/create",
    element: (
      <RequireAuthPage roles={["Company"]} page={<InternshipCreate />} />
    ),
  },
  {
    path: "/internship/edit/:id",
    element: <RequireAuthPage roles={["Company"]} page={<InternshipEdit />} />,
  },
  {
    path: "/internship/details/:id",
    element: <RequireAuthPage roles={[]} page={<InternshipDetails />} />,
  },
  {
    path: "/internship/applications",
    element: (
      <RequireAuthPage
        roles={["Company"]}
        page={<CompanyInternshipApplications />}
      />
    ),
  },
]);

const root = ReactDOM.createRoot(document.getElementById("root"));
root.render(
  <React.StrictMode>
    <RouterProvider router={router} />
  </React.StrictMode>
);

// If you want to start measuring performance in your app, pass a function
// to log results (for example: reportWebVitals(console.log))
// or send to an analytics endpoint. Learn more: https://bit.ly/CRA-vitals
reportWebVitals();
