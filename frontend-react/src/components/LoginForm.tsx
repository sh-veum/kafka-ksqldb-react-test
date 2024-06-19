import { baseUrl } from "@/lib/baseUrls";
import { LoginInfo } from "@/models/LoginInfo";
import { useForm } from "@tanstack/react-form";
import { useMutation } from "@tanstack/react-query";
import axios from "axios";
import { Label } from "./ui/label";
import { Input } from "./ui/input";
import { Spinner } from "./Spinner";
import { Alert, AlertDescription, AlertTitle } from "./ui/alert";
import { AlertCircle } from "lucide-react";
import { Button } from "./ui/button";

export const LoginForm = () => {
  const mutation = useMutation({
    mutationFn: (loginInfo: LoginInfo) => {
      return axios.post(`${baseUrl}/api/auction/insert_auction`, loginInfo);
    },
  });

  const form = useForm<LoginInfo>({
    defaultValues: {
      Email: "",
      Password: "",
    },
    onSubmit: (value) => {
      console.log(value.value);
      mutation.mutate({
        Email: value.value.Email,
        Password: value.value.Password,
      });
    },
  });

  return (
    <>
      <form>
        <form.Field
          name="Email"
          validators={{
            onChange: ({ value }) => {
              if (!value) {
                return "Email is required";
              }
            },
          }}
          children={(field) => (
            <div>
              <Label htmlFor={field.name}>Email</Label>
              <div className="relative">
                <Input
                  id="Email"
                  type="email"
                  value={field.state.value}
                  onChange={(e) => field.handleChange(e.target.value)}
                />
                {/* TODO: Backend validator */}
                {field.getMeta().isValidating && (
                  <div className="absolute right-2 top-1/2 transform -translate-y-1/2">
                    <Spinner />
                  </div>
                )}
              </div>
              {field.state.meta.errors && (
                <div className="text-red-500 text-sm mt-1">
                  {field.state.meta.errors}
                </div>
              )}
            </div>
          )}
        />
        <form.Field
          name="Password"
          validators={{
            onChange: ({ value }) => {
              if (!value) {
                return "Password is required";
              }
            },
          }}
          children={(field) => (
            <div>
              <Label htmlFor={field.name}>Password</Label>
              <div className="relative">
                <Input
                  id="Password"
                  type="password"
                  value={field.state.value}
                  onChange={(e) => field.handleChange(e.target.value)}
                />
                {/* TODO: Backend validator */}
                {field.getMeta().isValidating && (
                  <div className="absolute right-2 top-1/2 transform -translate-y-1/2">
                    <Spinner />
                  </div>
                )}
              </div>
              {field.state.meta.errors && (
                <div className="text-red-500 text-sm mt-1">
                  {field.state.meta.errors}
                </div>
              )}
            </div>
          )}
        />
        <form.Subscribe
          selector={(state) => state.errors}
          children={(errors) =>
            errors.length > 0 && (
              <Alert variant={"destructive"}>
                <AlertCircle className="h-4 w-4" />
                <AlertTitle>Error</AlertTitle>
                <AlertDescription>{errors}</AlertDescription>
              </Alert>
            )
          }
        />
        <form.Subscribe
          selector={(state) => [state.canSubmit, state.isValidating]}
          children={([canSubmit, isValidating]) => (
            <Button
              onClick={form.handleSubmit}
              disabled={!canSubmit || isValidating}
              className="w-full font-bold text-foreground mt-2"
            >
              Login
            </Button>
          )}
        />
      </form>
    </>
  );
};
