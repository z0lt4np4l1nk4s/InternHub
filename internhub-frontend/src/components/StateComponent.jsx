import React from "react";

export default function StateComponent({ state }) {
  return (
    <div
      style={{
        marginRight: "50px",
        padding: 6,
        borderRadius: "8px",
        backgroundColor:
          state.toLowerCase() === "accepted"
            ? "green"
            : state.toLowerCase() === "declined"
            ? "red"
            : "orange",
      }}
    >
      {state}
    </div>
  );
}
