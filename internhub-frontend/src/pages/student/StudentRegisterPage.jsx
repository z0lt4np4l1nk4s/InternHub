import React, { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";
import {
  Button,
  Form,
  Input,
  Loader,
  NavigationBar,
  SelectDropdown,
} from "../../components";
import { Student } from "../../models";
import {
  CountyService,
  StudentService,
  StudyAreaService,
} from "../../services";

export default function StudentRegisterPage() {
  const countyService = new CountyService();
  const studyAreaService = new StudyAreaService();
  const studentService = new StudentService();
  const [loading, setLoading] = useState(true);
  const [counties, setCounties] = useState([]);
  const [studyArea, setStudyAreas] = useState([]);
  const [studyAreaId, setStudyAreaId] = useState("");
  const [countyId, setCountyId] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    async function fetchData() {
      setCounties(await countyService.getAsync());
      setStudyAreas(await studyAreaService.getAsync());
      setLoading(false);
    }
    fetchData();
  }, []);

  if (loading) return <Loader />;
  return (
    <div>
      <NavigationBar />

      <div className="container">
        <div className="text-center">
          <h1>Student register</h1>
        </div>
        <div style={{ height: 40 }}></div>
        <Form
          onSubmit={async (e) => {
            e.preventDefault();
            if (e.target.password.value !== e.target.confirmpassword.value) {
              alert("The two passwords don't match!");
              return;
            }
            var student = new Student({
              firstName: e.target.firstName.value,
              lastName: e.target.lastName.value,
              email: e.target.email.value,
              phoneNumber: e.target.phoneNumber.value,
              address: e.target.address.value,
              description: e.target.description.value,
              password: e.target.password ? e.target.password.value : null,
              countyId: countyId,
              studyAreaId: studyAreaId,
            });
            const result = await studentService.postAsync(student);
            if (result) {
              navigate("/login");
            } else {
              alert(
                "An error occurred while registering your account... Please try again later!"
              );
            }
          }}
        >
          <Input name="email" text="Email:" type="email" required={true} />
          <Input
            type="password"
            name="password"
            text="Password:"
            minLength={6}
            required={true}
          />
          <Input
            type="password"
            name="confirmpassword"
            text="Confirm password:"
            minLength={6}
            required={true}
          />
          <Input name="firstName" text="First name:" required={true} />
          <Input name="lastName" text="Last name:" required={true} />
          <Input name="phoneNumber" text="Phone number:" required={true} />
          <SelectDropdown
            text={"County:"}
            placeholder={"Select county:"}
            name={"county"}
            list={counties}
            selectedId={countyId}
            onChange={(e) => setCountyId(e.target.value)}
          />
          <Input name="address" text="Address:" required={true} />
          <Input name="description" text="Description:" />
          <SelectDropdown
            text={"Study area:"}
            placeholder={"Select study area"}
            name={"studyarea"}
            list={studyArea}
            selectedId={studyAreaId}
            onChange={(e) => setStudyAreaId(e.target.value)}
          />
          <br></br>
          <div className="text-center">
            <Button buttonColor="primary" type="submit">
              Create
            </Button>
          </div>
        </Form>
      </div>
    </div>
  );
}
