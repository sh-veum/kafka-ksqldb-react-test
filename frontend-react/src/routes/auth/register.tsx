import { RegisterForm } from "@/components/RegisterForm";
import { createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/auth/register")({
  component: Register,
});

function Register() {
  return (
    <div className="mx-auto my-auto">
      <div className="flex flex-col space-y-2 text-center">
        <h1 className="text-2xl font-semibold tracking-tight">
          Create an account
        </h1>
        <p className="text-sm text-muted-foreground">
          Enter your email, username and password below to create your account
        </p>
      </div>
      <RegisterForm />
    </div>
  );
}
