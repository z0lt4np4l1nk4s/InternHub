import React, { useEffect, useState } from "react";
import { CountyService } from "../../services/CountyService";
import { Button, Input, MultiSelect } from "../index";

const InternshipFilter = ({ onFilter, onClearFilter, filter }) => {
  const [counties, setCounties] = useState(filter.counties || []);
  const [startDate, setStartDate] = useState(filter.startDate || "");
  const [endDate, setEndDate] = useState(filter.endDate || "");
  const [name, setName] = useState(filter.name || "");
  const [countiesOptions, setCountiesOptions] = useState([]);

  const countyService = new CountyService();

  const handleFilter = () => {
    const filterData = {
      counties: counties.map((county) => county.value),
      startDate: startDate || "",
      endDate: endDate || "",
      name: name || "",
    };
    onFilter(filterData);
  };

  const handleCancel = () => {
    setCounties([]);
    setEndDate("");
    setStartDate("");
    setName("");
    onClearFilter();
  };

  const fetchCountiesAsync = async () => {
    let counties = await countyService.getAsync();
    let mappedCounties = counties.map((county) => ({
      value: county.id,
      label: county.name,
    }));
    setCountiesOptions(mappedCounties);
    if (filter.counties) {
      setCounties(
        mappedCounties.filter((county) =>
          filter.counties.includes(county.value)
        )
      );
    }
  };

  useEffect(() => {
    fetchCountiesAsync();
  }, []);

  return (
    <div className="d-flex justify-content-center align-items-center">
      <div className="row">
        <div className="col-md-3">
          <Input
            text="Name:"
            value={name}
            onChange={(e) => setName(e.target.value)}
          />
        </div>

        <div className="col-md-3">
          <Input
            type="date"
            text="Start Date:"
            value={startDate}
            onChange={(e) => setStartDate(e.target.value)}
          />
        </div>

        <div className="col-md-3">
          <Input
            type="date"
            text="End Date:"
            value={endDate}
            onChange={(e) => setEndDate(e.target.value)}
          />
        </div>

        <div className="col-md-3">
          <MultiSelect
            text="Counties:"
            options={countiesOptions}
            value={counties}
            onChange={setCounties}
          />
        </div>
      </div>

      <div className="ml-3">
        <Button buttonColor="primary" onClick={handleFilter}>
          Filter
        </Button>
      </div>
      <div className="">
        <Button buttonColor="secondary" onClick={handleCancel}>
          Cancel
        </Button>
      </div>
    </div>
  );
};

export default InternshipFilter;
