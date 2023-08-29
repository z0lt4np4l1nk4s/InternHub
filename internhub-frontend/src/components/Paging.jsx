import React from "react";
import { Button } from "react-bootstrap";

export default function Paging({ currentPage, lastPage, onPageChanged }) {
  function handlePageChange(pageNumber) {
    if (
      pageNumber !== currentPage &&
      pageNumber >= 1 &&
      pageNumber <= lastPage
    ) {
      onPageChanged(pageNumber);
    }
  }

  return (
    <div className="d-flex justify-content-center align-items-center text-center my-5">
      <ul className="pagination text-light">
        <li
          className={`page-item ${currentPage === 1 ? "disabled" : ""}`}
          onClick={() => {
            handlePageChange(currentPage - 1);
          }}
        >
          <Button variant="link" className="page-link">
            Previous
          </Button>
        </li>
        {lastPage <= 5 && (
          <>
            {[...Array(lastPage)].map((x, i) => (
              <li
                key={i}
                className={`page-item${currentPage === i + 1 ? " active" : ""}`}
                onClick={() => handlePageChange(i + 1)}
              >
                <Button variant="link" className="page-link">
                  {i + 1}
                </Button>
              </li>
            ))}
          </>
        )}
        {lastPage > 5 && (
          <>
            {currentPage <= 2 && (
              <>
                {[...Array(3)].map((x, i) => (
                  <li
                    key={i}
                    className={`page-item${
                      currentPage === i + 1 ? " active" : ""
                    }`}
                    onClick={() => handlePageChange(i + 1)}
                  >
                    <Button variant="link" className="page-link">
                      {i + 1}
                    </Button>
                  </li>
                ))}
                <li className="page-item">
                  <span className="page-link">...</span>
                </li>
                <li
                  className={`page-item${
                    currentPage === lastPage ? " active" : ""
                  }`}
                  onClick={() => handlePageChange(lastPage)}
                >
                  <Button variant="link" className="page-link">
                    {lastPage}
                  </Button>
                </li>
              </>
            )}
            {currentPage > 2 && lastPage - 2 >= currentPage && (
              <>
                <li
                  className={`page-item${currentPage === 1 ? " active" : ""}`}
                  onClick={() => handlePageChange(1)}
                >
                  <Button variant="link" className="page-link">
                    1
                  </Button>
                </li>
                {currentPage - 2 > 1 && (
                  <li className="page-item">
                    <span className="page-link">...</span>
                  </li>
                )}
                {[...Array(3)].map((x, i) => (
                  <li
                    key={i}
                    className={`page-item${
                      currentPage === currentPage - 1 + i ? " active" : ""
                    }`}
                    onClick={() => handlePageChange(currentPage - 1 + i)}
                  >
                    <Button variant="link" className="page-link">
                      {currentPage - 1 + i}
                    </Button>
                  </li>
                ))}
                {currentPage + 2 < lastPage && (
                  <li className="page-item">
                    <span className="page-link">...</span>
                  </li>
                )}
                <li
                  className={`page-item${
                    currentPage === lastPage ? " active" : ""
                  }`}
                  onClick={() => handlePageChange(lastPage)}
                >
                  <Button variant="link" className="page-link">
                    {lastPage}
                  </Button>
                </li>
              </>
            )}
            {currentPage >= lastPage - 1 && (
              <>
                <li
                  className={`page-item${currentPage === 1 ? " active" : ""}`}
                  onClick={() => handlePageChange(1)}
                >
                  <Button variant="link" className="page-link">
                    1
                  </Button>
                </li>
                {currentPage - 2 > 1 && (
                  <li className="page-item">
                    <span className="page-link">...</span>
                  </li>
                )}
                {[...Array(3)].map((x, i) => (
                  <li
                    key={i}
                    className={`page-item${
                      currentPage === lastPage - 2 + i ? " active" : ""
                    }`}
                    onClick={() => handlePageChange(lastPage - 2 + i)}
                  >
                    <Button variant="link" className="page-link">
                      {lastPage - 2 + i}
                    </Button>
                  </li>
                ))}
              </>
            )}
          </>
        )}
        <li
          className={`page-item ${currentPage === lastPage ? "disabled" : ""}`}
          onClick={() => {
            if (currentPage < lastPage) handlePageChange(currentPage + 1);
          }}
        >
          <Button variant="link" className="page-link">
            Next
          </Button>
        </li>
      </ul>
    </div>
  );
}
