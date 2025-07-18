import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { ArrowLeft, Users } from 'lucide-react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import { useToast } from '@/hooks/use-toast';

export function JoinSettlement() {
  const navigate = useNavigate();
  const { toast } = useToast();
  const [inviteCode, setInviteCode] = useState('');
  const [loading, setLoading] = useState(false);

  const handleJoin = async () => {
    if (!inviteCode.trim()) {
      toast({
        title: "Error",
        description: "Please enter an invite code",
        variant: "destructive",
      });
      return;
    }

    setLoading(true);
    // Simulate API call
    setTimeout(() => {
      setLoading(false);
      toast({
        title: "Success",
        description: "Joined settlement successfully!",
      });
      navigate('/dashboard');
    }, 1000);
  };

  return (
    <div className="min-h-screen bg-gradient-hero">
      {/* Header */}
      <div className="bg-white border-b border-border p-4">
        <div className="flex items-center gap-3">
          <Button 
            variant="ghost" 
            size="icon"
            onClick={() => navigate('/dashboard')}
            className="shrink-0"
          >
            <ArrowLeft className="w-5 h-5" />
          </Button>
          
          <div className="flex-1">
            <h1 className="text-xl font-bold text-foreground">Join Settlement</h1>
            <p className="text-sm text-muted-foreground">Enter an invite code to join</p>
          </div>
        </div>
      </div>

      <div className="p-4 space-y-6">
        <Card className="shadow-card border-0">
          <CardHeader className="text-center">
            <div className="w-16 h-16 bg-primary-light rounded-full flex items-center justify-center mx-auto mb-4">
              <Users className="w-8 h-8 text-primary" />
            </div>
            <CardTitle className="text-xl">Join a Settlement</CardTitle>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="inviteCode">Invite Code</Label>
              <Input
                id="inviteCode"
                type="text"
                placeholder="Enter invite code"
                value={inviteCode}
                onChange={(e) => setInviteCode(e.target.value)}
                className="h-12 rounded-xl"
              />
            </div>
            
            <Button 
              onClick={handleJoin}
              disabled={loading}
              className="w-full h-12"
            >
              {loading ? 'Joining...' : 'Join Settlement'}
            </Button>
          </CardContent>
        </Card>

        <Card className="shadow-card border-0">
          <CardContent className="p-6">
            <h3 className="font-semibold mb-2">How to join a settlement</h3>
            <div className="space-y-2 text-sm text-muted-foreground">
              <p>1. Get an invite code from a settlement member</p>
              <p>2. Enter the code above and tap "Join Settlement"</p>
              <p>3. You'll be added to the settlement and can start adding expenses</p>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}