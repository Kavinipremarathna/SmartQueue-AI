export const API_BASE =
  import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5055";

export const AUTH_ENDPOINTS = {
  token: "/api/auth/token",
};

export async function fetchJson(url, options = {}) {
  const response = await fetch(url, options);
  if (!response.ok) {
    if (response.status === 401) {
      throw new Error("Unauthorized. Please login again.");
    }
    throw new Error(`Request failed (${response.status})`);
  }

  if (response.status === 204) {
    return null;
  }

  return response.json();
}

export function authHeaders(token, extra = {}) {
  return {
    "Content-Type": "application/json",
    Authorization: `Bearer ${token}`,
    ...extra,
  };
}
