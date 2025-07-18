const API_BASE_URL = 'https://localhost:5001/api/v1';

// Auth storage helpers
export const getAuthToken = () => localStorage.getItem('authToken');
export const setAuthToken = (token: string) => localStorage.setItem('authToken', token);
export const removeAuthToken = () => localStorage.removeItem('authToken');

// API client with auth header
const apiCall = async (endpoint: string, options: RequestInit = {}) => {
  const token = getAuthToken();
  const headers = {
    'Content-Type': 'application/json',
    ...(token && { Authorization: `Bearer ${token}` }),
    ...options.headers,
  };

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...options,
    headers,
  });

  if (!response.ok) {
    throw new Error(`API Error: ${response.status} ${response.statusText}`);
  }

  return response.json();
};

// Auth API
export const authApi = {
  login: async (email: string, password: string) => 
    apiCall('/Users/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    }),
  
  register: async (email: string, nickname: string, password: string) =>
    apiCall('/Users/register', {
      method: 'POST',
      body: JSON.stringify({ email, nickname, password }),
    }),
  
  getMe: async () => apiCall('/Users/me'),
};

// Settlements API
export const settlementsApi = {
  getAll: async (page: number = 1, pageSize: number = 10) =>
    apiCall(`/Settlements?Page=${page}&PageSize=${pageSize}`),
  
  getById: async (settlementId: string) =>
    apiCall(`/Settlements/${settlementId}`),
  
  create: async (name: string, participantNames: string[]) =>
    apiCall('/Settlements', {
      method: 'POST',
      body: JSON.stringify({ name, participantNames }),
    }),
  
  update: async (settlementId: string, name: string, participants: Array<{id: string | null, nickname: string}>) =>
    apiCall(`/Settlements/${settlementId}`, {
      method: 'PUT',
      body: JSON.stringify({ name, participants }),
    }),
  
  delete: async (settlementId: string) =>
    apiCall(`/Settlements/${settlementId}`, { method: 'DELETE' }),
  
  join: async (inviteCode: string) =>
    apiCall('/Settlements/join', {
      method: 'POST',
      body: JSON.stringify({ inviteCode }),
    }),
  
  leave: async (settlementId: string) =>
    apiCall(`/Settlements/${settlementId}/leave`, { method: 'POST' }),
  
  getExpenses: async (settlementId: string) =>
    apiCall(`/Settlements/${settlementId}/expenses`),
  
  getReimbursement: async (settlementId: string) =>
    apiCall(`/Settlements/${settlementId}/reimbursement`),
};

// Expenses API
export const expensesApi = {
  create: async (expense: {
    name: string;
    paymentDate: string;
    settlementId: string;
    payingParticipantId: string;
    allocations: Array<{ participantId: string; value: number }>;
  }) =>
    apiCall('/Expenses', {
      method: 'POST',
      body: JSON.stringify(expense),
    }),
  
  getById: async (expenseId: string) =>
    apiCall(`/Expenses/${expenseId}`),
  
  update: async (expenseId: string, expense: {
    title: string;
    paymentDate: string;
    payingParticipantId: string;
    allocations: Array<{ id?: string; participantId: string; value: number }>;
  }) =>
    apiCall(`/Expenses/${expenseId}`, {
      method: 'PUT',
      body: JSON.stringify(expense),
    }),
  
  delete: async (expenseId: string) =>
    apiCall(`/Expenses/${expenseId}`, { method: 'DELETE' }),
};