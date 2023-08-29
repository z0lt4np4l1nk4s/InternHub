import { useEffect, useState } from "react";
import { CompanyFilter } from "../../models";
import Button from "../Button";
import CheckBox from "../CheckBox";
import Form from "../Form";
import Input from "../Input";

export default function CompanyAdminFilter({
  onFilter,
  onClearFilter,
  filter,
}) {
  const [isActive, setIsActive] = useState(true);
  const [name, setName] = useState(filter.name || "");
  const [isAccepted, setIsAccepted] = useState(false);
  const [isFilterActive, setIsFilterActive] = useState(false);

  useEffect(() => {
    setIsFilterActive(filter.name || isAccepted || !isActive);
    if (filter.isAccepted !== null) setIsAccepted(filter.isAccepted);
    if (filter.isActive !== null) setIsActive(filter.isActive);
  }, []);

  return (
    <div className="container row justify-content-center align-items-center">
      <Form
        onSubmit={(e) => {
          e.preventDefault();
          const filter = new CompanyFilter({
            name: name,
            isActive,
            isAccepted,
          });
          onFilter(filter);
          setIsFilterActive(true);
        }}
      >
        <div className="row">
          <div className="col-4">
            <Input
              text="Name"
              name="name"
              value={name}
              onChange={(e) => setName(e.target.value)}
            />
          </div>
          <div className="col-4">
            <CheckBox
              text="Active"
              name="isactive"
              checked={isActive}
              onChange={(value) => {
                setIsActive(value);
              }}
            />
          </div>
          <div className="col-4">
            <CheckBox
              text="Accepted"
              name="isaccepted"
              checked={isAccepted}
              onChange={(value) => {
                setIsAccepted(value);
              }}
            />
          </div>
        </div>
        <Button type="submit" buttonColor="primary">
          Filter
        </Button>
        {isFilterActive && (
          <Button
            buttonColor="secondary"
            onClick={() => {
              setName("");
              setIsAccepted(false);
              setIsActive(true);
              setIsFilterActive(false);
              onClearFilter();
            }}
          >
            Clear filter
          </Button>
        )}
      </Form>
    </div>
  );
}
