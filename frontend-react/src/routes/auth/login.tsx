import { LoginForm } from "@/components/LoginForm";
import { Link, createFileRoute } from "@tanstack/react-router";

export const Route = createFileRoute("/auth/login")({
  component: Login,
});

function Login() {
  return (
    <div className="mx-auto my-auto">
      <div className="flex flex-col space-y-2 text-center">
        <h1 className="text-2xl font-semibold tracking-tight">Login</h1>
        <p className="text-sm text-muted-foreground">
          Enter your email and password below to login
        </p>
      </div>
      <LoginForm />
      <div>
        <p>
          Don't have an account?{" "}
          <Link
            to="/auth/register"
            className="underline text-blue-600 hover:text-blue-800 visited:text-purple-600"
          >
            Register
          </Link>
        </p>
      </div>
    </div>
  );
}
