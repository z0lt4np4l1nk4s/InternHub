import React, { useState, useEffect } from "react";
import { Form, useNavigate, useParams } from "react-router-dom";
import Button from "../../components/Button";
import { InternshipService, StudyAreaService } from "../../services";
import Input from "../../components/Input";
import { Internship } from "../../models";
import { NotFoundPage } from "..";
import Loader from "../../components/Loader";
import NavigationBar from "../../components/NavigationBar";
import { SelectDropdown } from "../../components";

export default function InternshipEdit() {
  const [internship, setInternship] = useState(null);
  const { id } = useParams();
  const [loading, setLoading] = useState(true);
  const [name, setName] = useState();
  const [startDate, setStartDate] = useState(new Date());
  const [endDate, setEndDate] = useState(new Date());
  const [description, setDescription] = useState();
  const [address, setAddress] = useState();
  const [studyAreas, setStudyAreas] = useState([]);
  const [studyAreaId, setStudyAreaId] = useState("");
  const navigate = useNavigate();

  const internshipService = new InternshipService();

  const fetchInternship = async () => {
    const internship = await internshipService.getByIdAsync(id);
    setInternship(internship);
    if (internship) {
      setName(internship.name);
      setStartDate(
        internshipService.convertToShorterDate(internship.startDate)
      );
      setEndDate(internshipService.convertToShorterDate(internship.endDate));
      setDescription(internship.description);
      setAddress(internship.address);
      setStudyAreaId(internship.studyAreaId);
    }
    setLoading(false);
  };

  useEffect(() => {
    fetchStudyAreas();
    fetchInternship();
  }, []);

  const handleNameChange = (e) => {
    setName(e.target.value);
  };

  const handleStartDate = (e) => {
    setStartDate(e.target.value);
  };
  const handleEndDate = (e) => {
    setEndDate(e.target.value);
  };

  const handleDescriptionChange = (e) => {
    setDescription(e.target.value);
  };

  const handleAddressChange = (e) => {
    setAddress(e.target.value);
  };

  const fetchStudyAreas = async () => {
    try {
      const studyAreasData = await new StudyAreaService().getAsync();
      setStudyAreas(studyAreasData);
    } catch (error) {
      console.log("Unable to fetch study areas.", error);
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const newInternship = new Internship({
      name,
      startDate,
      endDate,
      description,
      address,
      studyAreaId,
    });
    const result = await internshipService.updateAsync(id, newInternship);

    if (!result) {
      alert(
        "An error occured while updating the internship... Please try again later!"
      );
    } else {
      navigate("/internship/details/" + id);
    }
  };

  if (loading) return <Loader />;

  if (!internship) return <NotFoundPage />;

  return (
    <div className="container">
      <NavigationBar />
      <h1 className="text-center">Edit Internship</h1>
      <Form onSubmit={handleSubmit}>
        <div className="row justify-content-center mt-3">
          <div className="col-md-8">
            <div className="mb-3">
              <Input
                text="Name: "
                type="text"
                id="name"
                value={name || ""}
                onChange={handleNameChange}
                required={false}
              />
            </div>
            <div className="mb-3">
              <Input
                text="Start date: "
                type="date"
                id="startDate"
                value={startDate || ""}
                onChange={handleStartDate}
                required={false}
              />
            </div>
            <div className="mb-3">
              <Input
                text="End date: "
                type="date"
                id="endDate"
                value={endDate || ""}
                onChange={handleEndDate}
                required={false}
              />
            </div>
            <div className="mb-3">
              <Input
                text="Description: "
                type="text"
                id="description"
                value={description || ""}
                onChange={handleDescriptionChange}
                required={false}
              />
            </div>
            <div className="mb-3">
              <Input
                text="Address: "
                type="text"
                id="address"
                value={internship.company?.address || ""}
                onChange={handleAddressChange}
                required={false}
              />
            </div>
            <div className="mb-3">
              <SelectDropdown
                placeholder={"Select study area"}
                name={"studyArea"}
                list={studyAreas}
                text="Study Area:"
                selectedId={studyAreaId}
                onChange={(e) => setStudyAreaId(e.target.value)}
              />
            </div>
            {/* <div className="mb-3">
              <p className="text-center">
                <strong>Company:</strong>{" "}
                {internship.company ? internship.company.name : ""}
              </p>
              <p className="text-center">
                Check our{" "}
                <a
                  href={internship.company ? internship.company.website : ""}
                  target="_blank"
                  rel="noopener noreferrer"
                >
                  website
                </a>{" "}
                for more details.
              </p>
            </div> */}

            <div className="d-flex justify-content-center">
              <Button buttonColor="primary" type="submit">
                Save
              </Button>
            </div>
          </div>
        </div>
      </Form>
    </div>
  );
}
