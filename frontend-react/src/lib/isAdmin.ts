export const isAdmin: () => boolean = () => {
  return localStorage.getItem("role") === "ADMIN";
};
