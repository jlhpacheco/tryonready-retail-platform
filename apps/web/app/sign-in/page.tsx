import type { Metadata } from "next";
import { SignInForm } from "@/components/SignInForm";

export const metadata: Metadata = {
  title: "Judge Sign In",
};

export default function SignInPage() {
  return <SignInForm />;
}
