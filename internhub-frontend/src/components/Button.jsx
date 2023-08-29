import React from "react";

export default function Button({
  buttonColor,
  onClick,
  onSubmit,
  type,
  children,
  className,
  width,
}) {
  return (
    <button
      className={
        "btn btn-" + (buttonColor ?? "primary") + ` ${className ?? ""}`
      }
      onClick={onClick}
      onSubmit={onSubmit}
      type={type ?? "button"}
      style={{ width }}
    >
      {children}
    </button>
  );
}
