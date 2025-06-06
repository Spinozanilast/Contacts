import { isValidPhoneNumber } from "react-phone-number-input";
import { z } from "zod";

export const ContactSchema = z.object({
  name: z.string().min(2, { message: "Name must be at least 2 characters" }),
  mobilePhone: z
    .string()
    .min(5, { message: "Phone number must be at least 5 characters" })
    .refine((value) => isValidPhoneNumber(value), {
      message: "Invalid phone number format",
    }),
  jobTitle: z.string().optional(),
  birthDate: z.string().optional(),
});
