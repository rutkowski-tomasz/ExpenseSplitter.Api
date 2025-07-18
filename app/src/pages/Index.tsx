import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { AuthForm } from '@/components/AuthForm';
import { getAuthToken } from '@/lib/api';

const Index = () => {
  const [isAuthenticated, setIsAuthenticated] = useState(false);
  const [loading, setLoading] = useState(true);
  const navigate = useNavigate();

  useEffect(() => {
    const token = getAuthToken();
    setIsAuthenticated(!!token);
    setLoading(false);
    
    // Redirect to dashboard if authenticated
    if (token) {
      navigate('/dashboard');
    }
  }, [navigate]);

  const handleAuthSuccess = () => {
    setIsAuthenticated(true);
    navigate('/dashboard');
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gradient-hero flex items-center justify-center">
        <div className="text-center space-y-4">
          <div className="w-16 h-16 bg-gradient-primary rounded-full flex items-center justify-center mx-auto">
            <div className="w-8 h-8 border-2 border-white border-t-transparent rounded-full animate-spin" />
          </div>
          <div className="text-lg font-medium text-foreground">Loading ExpenseSplitter...</div>
        </div>
      </div>
    );
  }

  return <AuthForm onSuccess={handleAuthSuccess} />;
};

export default Index;
