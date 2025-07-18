import { useState } from 'react';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { useToast } from '@/hooks/use-toast';
import { authApi, setAuthToken } from '@/lib/api';
import { Eye, EyeOff, LogIn, UserPlus } from 'lucide-react';

interface AuthFormProps {
  onSuccess: () => void;
}

export function AuthForm({ onSuccess }: AuthFormProps) {
  const [isLogin, setIsLogin] = useState(true);
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [formData, setFormData] = useState({
    email: '',
    nickname: '',
    password: '',
  });
  const { toast } = useToast();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);

    try {
      if (isLogin) {
        const response = await authApi.login(formData.email, formData.password);
        setAuthToken(response.token);
        toast({
          title: "Welcome back!",
          description: "You've been logged in successfully.",
        });
      } else {
        await authApi.register(formData.email, formData.nickname, formData.password);
        const loginResponse = await authApi.login(formData.email, formData.password);
        setAuthToken(loginResponse.token);
        toast({
          title: "Account created!",
          description: "Welcome to ExpenseSplitter!",
        });
      }
      onSuccess();
    } catch (error) {
      toast({
        title: "Error",
        description: error instanceof Error ? error.message : "Something went wrong",
        variant: "destructive",
      });
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gradient-hero flex items-center justify-center p-4">
      <Card className="w-full max-w-md shadow-card border-0">
        <CardHeader className="text-center space-y-2">
          <div className="w-16 h-16 bg-gradient-primary rounded-full flex items-center justify-center mx-auto mb-4">
            <span className="text-2xl font-bold text-white">ES</span>
          </div>
          <CardTitle className="text-2xl font-bold">
            {isLogin ? 'Welcome Back' : 'Join ExpenseSplitter'}
          </CardTitle>
          <CardDescription>
            {isLogin ? 'Sign in to your account' : 'Create your account to get started'}
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} className="space-y-4">
            <div className="space-y-3">
              <Input
                type="email"
                placeholder="Email address"
                value={formData.email}
                onChange={(e) => setFormData(prev => ({ ...prev, email: e.target.value }))}
                required
                className="h-12 rounded-xl border-border focus:border-primary"
              />
              
              {!isLogin && (
                <Input
                  type="text"
                  placeholder="Nickname"
                  value={formData.nickname}
                  onChange={(e) => setFormData(prev => ({ ...prev, nickname: e.target.value }))}
                  required
                  className="h-12 rounded-xl border-border focus:border-primary"
                />
              )}
              
              <div className="relative">
                <Input
                  type={showPassword ? 'text' : 'password'}
                  placeholder="Password"
                  value={formData.password}
                  onChange={(e) => setFormData(prev => ({ ...prev, password: e.target.value }))}
                  required
                  className="h-12 rounded-xl border-border focus:border-primary pr-12"
                />
                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  className="absolute right-1 top-1 h-10 w-10"
                  onClick={() => setShowPassword(!showPassword)}
                >
                  {showPassword ? <EyeOff size={18} /> : <Eye size={18} />}
                </Button>
              </div>
            </div>

            <Button
              type="submit"
              className="w-full"
              disabled={loading}
            >
              {loading ? (
                <div className="w-5 h-5 border-2 border-white border-t-transparent rounded-full animate-spin" />
              ) : (
                <>
                  {isLogin ? <LogIn className="mr-2" size={18} /> : <UserPlus className="mr-2" size={18} />}
                  {isLogin ? 'Sign In' : 'Create Account'}
                </>
              )}
            </Button>
          </form>

          <div className="mt-6 text-center">
            <button
              type="button"
              className="text-sm text-muted-foreground hover:text-primary transition-colors"
              onClick={() => setIsLogin(!isLogin)}
            >
              {isLogin ? "Don't have an account? Sign up" : "Already have an account? Sign in"}
            </button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}