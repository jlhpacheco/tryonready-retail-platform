# Demo Script

All people, businesses, products, and identifiers in this script are fictional.

## One continuous 1-3 minute walkthrough

1. Open TryOnReady and state: “The retailer pays; the guest receives virtual
   try-on as a free courtesy and convenience.”
2. Select **Judge Sign In**, choose **Retailer**, and sign in with the private
   judge credentials.
3. Submit the prepared Luna & Thread application for fictional owner Elena
   Rivera.
4. Continue to Product Readiness and upload
   `samples/synthetic/garments/moonlight-blazer.png`.
5. Point out the 1254 x 1254 server-validated image and save the garment.
6. Return to Judge Sign In, choose **Administrator**, and sign in.
7. Approve Luna & Thread, inspect the garment metadata, and approve the
   Moonlight Blazer.
8. Continue to Consumer Try-On. Point out **API key in browser: Never**.
9. Upload `samples/synthetic/customers/marisol-lopez-source.png`, check consent,
   and generate the result once.
10. Show the provider state, succeeded result, and API-unit count.
11. Submit the unchanged request again and show **Duplicate request
    prevented**.
12. Open the dashboard and show completed try-ons without customer photographs.

## What to say about live versus simulation

Automated tests use the provider simulation and spend zero units. The live
adapter uses the same server workflow. The private YouCam key is configured
only as a server secret, and the controlled live evidence must be recorded in
`IMPLEMENTATION-LOG.md` without recording the key or signed URLs.

The full manual values, credentials, catalog, customer images, expected
results, and troubleshooting steps are in `JUDGE-TESTING-GUIDE.md` and the
Word/PDF guide under `docs/judge`.
