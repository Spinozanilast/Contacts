import { useMutation, useQuery } from "@tanstack/react-query";
import toast from "react-hot-toast";
import type {
  Contact,
  ContactsResponse,
  ContactsParams,
  SaveContactDto,
  UpdateContactDto,
} from "@/types/domain/contact";
import { api } from "@/lib/api";

export function useContacts(params: ContactsParams) {
  return useQuery({
    queryKey: ["contacts", params],
    queryFn: async () => {
      const response = await api.get<ContactsResponse>("/contacts", {
        params: {
          pageNumber: params.pageNumber,
          pageSize: params.pageSize,
          nameSearch: params.nameSearch,
          jobTitleSearch:
            params.jobTitleSearch == "all" || params.jobTitleSearch == ""
              ? null
              : params.jobTitleSearch,
        },
      });
      return response.data;
    },
    keepPreviousData: true,
    staleTime: 1 * 60 * 1000,
  });
}

export function useContact(id: string) {
  return useQuery({
    queryKey: ["contact", id],
    queryFn: async () => {
      const response = await api.get<Contact>(`/contacts/${id}`);
      return response.data;
    },
    enabled: !!id,
    staleTime: 5 * 60 * 1000,
  });
}

export function useCreateContact() {
  return useMutation({
    mutationFn: async (contact: SaveContactDto) => {
      const response = await api.post<void>(
        "/contacts",
        nullifyContactProps(contact),
      );
      return response.data;
    },
    onSuccess: () => {
      toast.success("Contact created successfully");
    },
    onError: (error) => {
      console.error("Error creating contact:", error);
      toast.error("Failed to create contact");
    },
  });
}

export function useUpdateContact() {
  return useMutation({
    mutationFn: async (contact: UpdateContactDto) => {
      const response = await api.put<void>(
        `/contacts/${contact.id}`,
        nullifyContactProps(contact),
      );
      return response.data;
    },
    onSuccess: () => {
      toast.success("Contact updated successfully");
    },
    onError: (error) => {
      console.error("Error updating contact:", error);
      toast.error("Failed to update contact");
    },
  });
}

export function useDeleteContact() {
  return useMutation({
    mutationFn: async (id: string) => {
      const response = await api.delete<void>(`/contacts/${id}`);
      return response.data;
    },
    onSuccess: () => {
      toast.success("Contact deleted successfully");
    },
    onError: (error) => {
      console.error("Error deleting contact:", error);
      toast.error("Failed to delete contact");
    },
  });
}

function nullifyContactProps(dto: SaveContactDto) {
  return {
    ...dto,
    jobTitle: dto.jobTitle === "" ? null : dto.jobTitle,
    birthDate: dto.birthDate === "" ? null : dto.birthDate,
  };
}
