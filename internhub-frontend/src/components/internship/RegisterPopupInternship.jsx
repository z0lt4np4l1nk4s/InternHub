import React, { useState } from "react";
import { Modal } from "react-bootstrap";
import { InternshipApplicationService } from "../../services";
import { useParams } from "react-router-dom";

const RegisterPopupInternship = ({
  showPopup,
  handleClose,
  handleApplySuccess,
}) => {
  const [applyMessage, setApplyMessage] = useState("");
  const { id } = useParams();
  const [closePopup, setClosePopup] = useState(false);

  const internshipApplicationService = new InternshipApplicationService();

  const handleApply = async () => {
    if (await internshipApplicationService.postAsync(id, applyMessage)) {
      setClosePopup(true);
      handleApplySuccess();
    }
  };

  return (
    <Modal centered show={!closePopup && showPopup} onHide={handleClose}>
      <Modal.Header closeButton>
        <Modal.Title className="text-center">
          Write something about yourself for this application.
        </Modal.Title>
      </Modal.Header>
      <Modal.Body>
        <textarea
          onChange={(e) => {
            setApplyMessage(e.target.value);
          }}
          className="form-control"
          rows="3"
          placeholder="Enter text here"
        ></textarea>
        <button onClick={handleApply}>Apply</button>
      </Modal.Body>
    </Modal>
  );
};

export default RegisterPopupInternship;
