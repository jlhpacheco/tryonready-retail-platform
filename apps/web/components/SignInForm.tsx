"use client";

import { FormEvent, useState } from "react";

type LoginResult = {
  displayName: string;
  role: string;
  next: string;
};

const accounts = {
  retailer: {
    label: "Retailer",
    username: "retailer@tryonready.demo",
    description: "Submit Luna & Thread and add its garment.",
  },
  administrator: {
    label: "Administrator",
    username: "admin@tryonready.demo",
    description: "Approve the boutique and garment, then view results.",
  },
};

export function SignInForm() {
  const [account, setAccount] = useState<keyof typeof accounts>("retailer");
  const [username, setUsername] = useState(accounts.retailer.username);
  const [message, setMessage] = useState<string | null>(null);
  const [isSubmitting, setIsSubmitting] = useState(false);

  function chooseAccount(next: keyof typeof accounts) {
    setAccount(next);
    setUsername(accounts[next].username);
    setMessage(null);
  }

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    setMessage(null);
    setIsSubmitting(true);
    const form = new FormData(event.currentTarget);

    try {
      const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
          username: form.get("username"),
          password: form.get("password"),
        }),
      });

      if (!response.ok) {
        throw new Error(
          "The username or password was not recognized. Use the exact judge credentials.",
        );
      }

      const result = (await response.json()) as LoginResult;
      window.location.assign(result.next);
    } catch (error) {
      setMessage(
        error instanceof Error ? error.message : "Sign-in could not continue.",
      );
      setIsSubmitting(false);
    }
  }

  return (
    <section className="sign-in-shell">
      <div className="sign-in-intro">
        <p className="eyebrow">Judge demo access</p>
        <h1>Choose the role for the next step.</h1>
        <p>
          Retailer and administrator actions are separated. Guest customers do
          not sign in.
        </p>
      </div>

      <form className="sign-in-form" onSubmit={handleSubmit}>
        <div className="role-picker" role="group" aria-label="Demo role">
          {(Object.keys(accounts) as Array<keyof typeof accounts>).map(
            (key) => (
              <button
                className={account === key ? "role-active" : ""}
                type="button"
                key={key}
                onClick={() => chooseAccount(key)}
              >
                <strong>{accounts[key].label}</strong>
                <span>{accounts[key].description}</span>
              </button>
            ),
          )}
        </div>

        <label>
          <span>Username</span>
          <input
            name="username"
            type="email"
            value={username}
            onChange={(event) => setUsername(event.target.value)}
            autoComplete="username"
            required
          />
        </label>

        <label>
          <span>Password</span>
          <input
            name="password"
            type="password"
            autoComplete="current-password"
            required
          />
        </label>

        <p className="privacy-note">
          Passwords are provided in the private judge testing instructions.
        </p>

        <button
          className="button button-primary"
          type="submit"
          disabled={isSubmitting}
        >
          {isSubmitting ? "Signing in…" : `Continue as ${accounts[account].label}`}
        </button>

        {message ? (
          <div className="readiness-result readiness-error" role="alert">
            <h2>Sign-in unsuccessful</h2>
            <p>{message}</p>
          </div>
        ) : null}
      </form>
    </section>
  );
}
