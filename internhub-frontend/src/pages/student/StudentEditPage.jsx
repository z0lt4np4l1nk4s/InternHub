import React, { useEffect, useState } from "react";
import { useNavigate, useParams, Link } from "react-router-dom";
import {
  Button,
  Form,
  Input,
  Loader,
  SelectDropdown,
  NavigationBar,
} from "../../components";
import { Student } from "../../models";
import {
  CountyService,
  StudentService,
  StudyAreaService,
  LoginService,
} from "../../services";
import NotFoundPage from "../NotFoundPage";

export default function StudentEditPage() {
  const studentService = new StudentService();
  const countyService = new CountyService();
  const studyAreaService = new StudyAreaService();
  const loginService = new LoginService();
  const [loading, setLoading] = useState(true);
  const [student, setStudent] = useState({});
  const [counties, setCounties] = useState([]);
  const [studyArea, setStudyAreas] = useState([]);
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [description, setDescription] = useState("");
  const [address, setAddress] = useState("");
  const [countyId, setCountyId] = useState("");
  const [studyAreaId, setStudyAreaId] = useState("");
  const navigate = useNavigate();
  const params = useParams();

  useEffect(() => {
    async function fetchData() {
      setCounties(await countyService.getAsync());
      setStudyAreas(await studyAreaService.getAsync());
      setLoading(false);
    }
    getStudent();
    fetchData();
  }, []);

  async function getStudent() {
    await studentService.getByIdAsync(params.id).then((student) => {
      setLoading(false);
      setStudent(student);
      if (student) {
        setFirstName(student.firstName);
        setLastName(student.lastName);
        setPhoneNumber(student.phoneNumber);
        setDescription(student.description);
        setAddress(student.address);
        setCountyId(student.countyId);
        setStudyAreaId(student.studyAreaId);
      }
    });
  }

  if (loading) return <Loader />;
  if (!student) return <NotFoundPage />;

  return (
    <div>
      <NavigationBar />

      <div className="container">
        <div className="text-center">
          <h1>Edit student</h1>
        </div>
        <Form
          onSubmit={async (e) => {
            e.preventDefault();
            var newStudent = new Student({
              firstName: firstName,
              lastName: lastName,
              phoneNumber: phoneNumber,
              address: address,
              description: description,
              countyId: countyId,
              studyAreaId: studyAreaId,
            });
            const result = await studentService.updateAsync(
              student.id,
              newStudent
            );
            if (result) {
              navigate("/student/details/" + student.id);
            } else
              alert(
                "An error occured while updating... Please try again later!"
              );
          }}
        >
          <Input
            name="firstName"
            text="First name:"
            value={firstName}
            onChange={(e) => setFirstName(e.target.value)}
          />
          <Input
            name="lastName"
            text="Last name:"
            value={lastName}
            onChange={(e) => setLastName(e.target.value)}
          />
          <Input
            name="phoneNumber"
            text="Phone number:"
            value={phoneNumber}
            onChange={(e) => setPhoneNumber(e.target.value)}
          />
          <SelectDropdown
            text={"County:"}
            placeholder={"Select county"}
            name={"county"}
            list={counties}
            onChange={(e) => setCountyId(e.target.value)}
            selectedId={countyId}
          />
          <Input
            name="address"
            text="Address:"
            value={address}
            onChange={(e) => setAddress(e.target.value)}
          />
          <Input
            name="description"
            text="Description:"
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          />
          <SelectDropdown
            text={"Study area:"}
            placeholder={"Select study area"}
            name={"studyArea"}
            list={studyArea}
            selectedId={studyAreaId}
            onChange={(e) => setStudyAreaId(e.target.value)}
          />
          <br></br>
          <Button buttonColor="primary" type="submit">
            Save
          </Button>
        </Form>
      </div>
    </div>
  );
}
